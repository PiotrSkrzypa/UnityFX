using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Alchemy.Inspector;
using System.Linq;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXSequence : IFXComponent
    {
        [SerializeField] bool canPlayWhenAlreadyPlaying;
        [SerializeField] bool rewindOnCancel = false;
        [SerializeField][ShowIf("reverseOnCancel")] float reverseSpeedMultiplier = 1f;
        FXTiming IFXComponent.Timing => SequenceTiming;
        public FXSequenceTiming SequenceTiming = new FXSequenceTiming();
        [SerializeField][SerializeReference] IFXComponent[] components;
        public IFXComponent[] Components { get => components; }

        public FXPlaybackStateID CurrentState => stateMachine.CurrentState;

        bool IFXComponent.CanPlayWhenAlreadyPlaying => canPlayWhenAlreadyPlaying;
        bool IFXComponent.ReverseOnCancel => rewindOnCancel;
        float IFXComponent.ReverseSpeedMultiplier => reverseSpeedMultiplier;


        public UnityEvent OnPlay;
        public UnityEvent OnCompleted;
        public UnityEvent OnCancelled;

        CancellationTokenSource cts;
        protected bool initialized;
        private FXPlaybackStateMachine stateMachine = new FXPlaybackStateMachine();
        int currentSequenceIndex;

        public void Initialize()
        {
        }
        [Button]
        public void Play()
        {
            Play(SequenceTiming.PlaybackSpeed).Forget();
        }
        public async UniTask Play(float inheritedSpeed = 1f)
        {
            if (stateMachine.CurrentState != FXPlaybackStateID.Idle)
            {
                if (!canPlayWhenAlreadyPlaying)
                {
                    Debug.LogWarning($"[FX] Cannot play {this} when already playing.");
                    return;
                }
                Stop();
            }
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            SequenceTiming.RecalculateDuration(components);
            await Play(token);
        }
        public async UniTask Play(CancellationToken token = default, float inheritedSpeed = 1f)
        {
            try
            {
                SequenceTiming.PlayCount = 0;
                OnPlay?.Invoke();
                SetState(FXPlaybackStateID.WaitingToStart);
                float effectiveSpeed = SequenceTiming.PlaybackSpeed * inheritedSpeed;

                if (effectiveSpeed > 0)
                {
                    await DelaySeconds(SequenceTiming.InitialDelay, effectiveSpeed, token, SequenceTiming.TimeScaleIndependent);
                }

                int repeatCount = SequenceTiming.RepeatForever ? int.MaxValue : Mathf.Max(1, SequenceTiming.NumberOfRepeats);
                for (int i = 0; i < repeatCount; i++)
                {
                    SetState(FXPlaybackStateID.Playing);
                    SequenceTiming.PlayCount++;
                    switch (SequenceTiming.PlayMode)
                    {
                        case SequencePlayMode.Sequential:
                            await PlaySequential(token, effectiveSpeed);
                            break;
                        case SequencePlayMode.Parallel:
                        default:
                            await PlayParallel(token, effectiveSpeed);
                            break;
                    }

                    if (i < repeatCount - 1)
                    {
                        SetState(FXPlaybackStateID.RepeatingDelay);
                        await DelaySeconds(SequenceTiming.DelayBetweenRepeats, effectiveSpeed, token, SequenceTiming.TimeScaleIndependent);
                    }
                }
                SetState(FXPlaybackStateID.Completed);
                OnCompleted?.Invoke();
                if (SequenceTiming.CooldownDuration > 0)
                {
                    SetState(FXPlaybackStateID.Cooldown);
                    await DelaySeconds(SequenceTiming.CooldownDuration, effectiveSpeed, token, SequenceTiming.TimeScaleIndependent);
                }
                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                if (stateMachine.CurrentState != FXPlaybackStateID.Cancelled || !rewindOnCancel)
                    throw;

                SetState(FXPlaybackStateID.Rewinding);
                await Rewind(inheritedSpeed);
            }
            finally
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;
            }
        }
        private async UniTask DelaySeconds(float duration, float speed, CancellationToken token, bool unscaled = false)
        {
            if (duration <= 0f) return;

            var milliseconds = (int)((duration / speed) * 1000);
            var type = unscaled ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
            await UniTask.Delay(milliseconds, type, PlayerLoopTiming.Update, token);
        }
        private async UniTask PlayParallel(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            CancellationToken defaultToken = default;
            if (components != null)
            {
                List<UniTask> tasks = new List<UniTask>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    UniTask task = components[i].Play(inheritedSpeed);
                    if (components[i].Timing.ContributeToTotalDuration)
                    {
                        tasks.Add(task);
                    }
                }
                if (cancellationToken != defaultToken)
                {
                    await UniTask.WhenAll(tasks).AttachExternalCancellation(cancellationToken);
                }
                else
                {
                    await UniTask.WhenAll(tasks);
                }
            }
        }

        private async UniTask PlaySequential(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            if (components != null)
            {
                List<UniTask> tasks = new List<UniTask>();
                var playList = inheritedSpeed > 0 ? Components : Components.Reverse().ToArray();
                for (int i = 0; i < playList.Length; i++)
                {
                    currentSequenceIndex = i;
                    playList[i].Initialize();
                    UniTask task = playList[i].Play(inheritedSpeed).AttachExternalCancellation(cancellationToken);
                    await task;
                }
            }
        }
        private async UniTask Rewind(float inheritedSpeed = 1f)
        {
            SequenceTiming.PlayCount--;
            float effectiveReverseSpeed = SequenceTiming.PlaybackSpeed * inheritedSpeed * reverseSpeedMultiplier;
            float effectiveSpeedAbs = Mathf.Abs(effectiveReverseSpeed);
            float calculatedInitialDelay = SequenceTiming.InitialDelay / effectiveSpeedAbs;
            float calculatedDelayBetweenRepeats = SequenceTiming.DelayBetweenRepeats / effectiveSpeedAbs;
            float calculatedCooldownDuration = SequenceTiming.CooldownDuration > 0 ? SequenceTiming.CooldownDuration / effectiveSpeedAbs : 0;

            switch (SequenceTiming.PlayMode)
            {
                case SequencePlayMode.Sequential:

                    if (components != null)
                    {
                        List<UniTask> tasks = new List<UniTask>();
                        for (int i = currentSequenceIndex; i >= 0; i--)
                        {
                            IFXComponent iFXComponent = components[i];
                            if (i == currentSequenceIndex)
                            {
                                iFXComponent.Cancel();
                            }
                            else
                            {
                                iFXComponent.Play(effectiveReverseSpeed).Forget();
                            }
                            UniTask task = UniTask.WaitUntil(()=>iFXComponent.CurrentState==FXPlaybackStateID.Idle);
                            await task;
                        }
                    }
                    break;

                case SequencePlayMode.Parallel:
                default:
                    if (components != null)
                    {
                        List<UniTask> tasks = new List<UniTask>();
                        for (int i = 0; i < components.Length; i++)
                        {
                            IFXComponent fXComponent = components[i];
                            fXComponent.Cancel();
                            UniTask task = UniTask.WaitUntil(()=>fXComponent.CurrentState==FXPlaybackStateID.Idle);
                            if (fXComponent.Timing.ContributeToTotalDuration)
                            {
                                tasks.Add(task);
                            }
                        }
                        await UniTask.WhenAll(tasks);
                    }
                    break;
            }

            while (SequenceTiming.PlayCount > 0)
            {
                SetState(FXPlaybackStateID.RepeatingDelay);
                await UniTask.Delay((int)( calculatedDelayBetweenRepeats * 1000 ),
                    SequenceTiming.TimeScaleIndependent ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime);
                SetState(FXPlaybackStateID.Rewinding);
                switch (SequenceTiming.PlayMode)
                {
                    case SequencePlayMode.Sequential:
                        await PlaySequential(default, effectiveReverseSpeed);
                        break;
                    case SequencePlayMode.Parallel:
                    default:
                        await PlayParallel(default, effectiveReverseSpeed);
                        break;
                }
                SequenceTiming.PlayCount--;
            }
            SetState(FXPlaybackStateID.Completed);
            OnCompleted?.Invoke();
            SetState(FXPlaybackStateID.Idle);
        }
        [Button]
        public void Stop()
        {
            if (!SetState(FXPlaybackStateID.Stopped) || components == null)
            {
                return;
            }

            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Stop();
            }
            SetState(FXPlaybackStateID.Idle);
            OnCancelled?.Invoke();
        }
        [Button]
        public void Reset()
        {
            if (components == null)
            {
                return;
            }
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Reset();
            }
            stateMachine.ForceSet(FXPlaybackStateID.Idle);
        }
        private bool SetState(FXPlaybackStateID newState)
        {
            return stateMachine.TryTransitionTo(newState);
        }
        [Button]
        public void Cancel()
        {
            if (!rewindOnCancel || stateMachine.CurrentState != FXPlaybackStateID.Playing)
            {
                Stop();
                return;
            }
            SetState(FXPlaybackStateID.Cancelled);
            cts?.Cancel();
        }
    }
    public enum SequencePlayMode
    {
        Parallel = 0,
        Sequential = 1
    }
}

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
        [SerializeField] PlaybackSpeed playbackSpeedSettings = new PlaybackSpeed { speed = 1f, rewindSpeed = -1f };
        FXTiming IFXComponent.Timing => SequenceTiming;
        public FXSequenceTiming SequenceTiming = new FXSequenceTiming();
        [SerializeField][SerializeReference] IFXComponent[] components;
        public IFXComponent[] Components { get => components; }

        public FXPlaybackStateID CurrentState => stateMachine.CurrentState;

        bool IFXComponent.CanPlayWhenAlreadyPlaying => canPlayWhenAlreadyPlaying;
        bool IFXComponent.RewindOnCancel => rewindOnCancel;
        float IFXComponent.RewindSpeedMultiplier => playbackSpeedSettings.rewindSpeed;


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
            Play(new PlaybackSpeed(1f, 1f)).Forget();
        }
        public async UniTask Play(PlaybackSpeed inheritedplaybackSpeed)
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
            PlaybackSpeed combinedPlaybackSpeed = playbackSpeedSettings * inheritedplaybackSpeed;

            await InternalPlay(combinedPlaybackSpeed, token);
        }
        private async UniTask InternalPlay(PlaybackSpeed playbackSpeed, CancellationToken token = default)
        {
            try
            {
                SequenceTiming.PlayCount = 0;
                OnPlay?.Invoke();
                SetState(FXPlaybackStateID.WaitingToStart);

                await DelaySeconds(SequenceTiming.InitialDelay, playbackSpeed.SpeedAbs, token, SequenceTiming.TimeScaleIndependent);

                int repeatCount = SequenceTiming.RepeatForever ? int.MaxValue : Mathf.Max(1, SequenceTiming.NumberOfRepeats);
                for (int i = 0; i < repeatCount; i++)
                {
                    SetState(FXPlaybackStateID.Playing);
                    SequenceTiming.PlayCount++;
                    switch (SequenceTiming.PlayMode)
                    {
                        case SequencePlayMode.Sequential:
                            await PlaySequential(token, playbackSpeed).AttachExternalCancellation(token);
                            break;
                        case SequencePlayMode.Parallel:
                        default:
                            await PlayParallel(token, playbackSpeed).AttachExternalCancellation(token);
                            break;
                    }

                    if (i < repeatCount - 1)
                    {
                        SetState(FXPlaybackStateID.RepeatingDelay);
                        await DelaySeconds(SequenceTiming.DelayBetweenRepeats, playbackSpeed.SpeedAbs, token, SequenceTiming.TimeScaleIndependent);
                    }
                }
                SetState(FXPlaybackStateID.Completed);
                OnCompleted?.Invoke();
                if (SequenceTiming.CooldownDuration > 0)
                {
                    SetState(FXPlaybackStateID.Cooldown);
                    await DelaySeconds(SequenceTiming.CooldownDuration, playbackSpeed.SpeedAbs, token, SequenceTiming.TimeScaleIndependent);
                }
                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                if (stateMachine.CurrentState != FXPlaybackStateID.Cancelled || !rewindOnCancel)
                    throw;

                SetState(FXPlaybackStateID.Rewinding);
                await Rewind(playbackSpeed);
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
        private async UniTask PlayParallel(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            CancellationToken defaultToken = default;
            if (components != null)
            {
                List<UniTask> tasks = new List<UniTask>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    UniTask task = components[i].Play(playbackSpeed);
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

        private async UniTask PlaySequential(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            if (components != null)
            {
                List<UniTask> tasks = new List<UniTask>();
                var playList = playbackSpeed.speed > 0 ? Components : Components.Reverse().ToArray();
                for (int i = 0; i < playList.Length; i++)
                {
                    currentSequenceIndex = i;
                    playList[i].Initialize();
                    UniTask task = playList[i].Play(playbackSpeed).AttachExternalCancellation(cancellationToken);
                    await task;
                }
            }
        }
        private async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            SequenceTiming.PlayCount--;

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
                                iFXComponent.Play(playbackSpeed.Flip()).Forget();
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
                await DelaySeconds(SequenceTiming.DelayBetweenRepeats, playbackSpeed.RewindSpeedAbs, default, SequenceTiming.TimeScaleIndependent);

                SetState(FXPlaybackStateID.Rewinding);
                switch (SequenceTiming.PlayMode)
                {
                    case SequencePlayMode.Sequential:
                        await PlaySequential(default, playbackSpeed.Flip());
                        break;
                    case SequencePlayMode.Parallel:
                    default:
                        await PlayParallel(default, playbackSpeed.Flip());
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

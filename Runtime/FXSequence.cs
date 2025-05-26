using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Alchemy.Inspector;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public  class FXSequence : IFXComponent
    {
        [SerializeField] SequencePlayMode playMode = SequencePlayMode.Parallel;
        public bool CanPlayWhenAlreadyPlaying;
        FXTiming IFXComponent.Timing => SequenceTiming;
        public FXSequenceTiming SequenceTiming;
        [SerializeField][SerializeReference] IFXComponent[] components;
        public IFXComponent[] Components { get => components; }
        public SequencePlayMode PlayMode { get => playMode; }

        public UnityEvent OnPlay;
        public UnityEvent OnCompleted;
        public UnityEvent OnCancelled;

        CancellationTokenSource cts;
        protected bool initialized;
        private FXPlaybackStateMachine stateMachine = new FXPlaybackStateMachine();


        public void Initialize()
        {
        }
        [Button]
        public async UniTask Play()
        {
            if (stateMachine.CurrentState != FXPlaybackStateID.Idle)
            {
                if (!CanPlayWhenAlreadyPlaying)
                {
                    Debug.LogWarning($"[FX] Cannot play {this} when already playing.");
                    return;
                }
                Stop();
            }
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            SequenceTiming.RecalculateDuration(components, playMode);
            await Play(token);
        }
        public async UniTask Play(CancellationToken token = default)
        {
            try
            {
                OnPlay?.Invoke();
                SetState(FXPlaybackStateID.WaitingToStart);
                await UniTask.Delay((int)( SequenceTiming.InitialDelay * 1000 ), cancellationToken: token);

                int repeatCount = SequenceTiming.RepeatForever ? int.MaxValue : Mathf.Max(1, SequenceTiming.NumberOfRepeats);
                for (int i = 0; i < repeatCount; i++)
                {
                    SetState(FXPlaybackStateID.Playing);
                    switch (playMode)
                    {
                        case SequencePlayMode.Sequential:
                            await PlaySequential(token);
                            break;
                        case SequencePlayMode.Parallel:
                        default:
                            await PlayParallel(token);
                            break;
                    }

                    if (i < repeatCount - 1)
                    {
                        SetState(FXPlaybackStateID.RepeatingDelay);
                        await UniTask.Delay((int)( SequenceTiming.DelayBetweenRepeats * 1000 ),
                            SequenceTiming.TimeScaleIndependent ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime,
                            cancellationToken: token);
                    }
                }
                SetState(FXPlaybackStateID.Completed);
                OnCompleted?.Invoke();
                if (SequenceTiming.CooldownDuration > 0)
                {
                    SetState(FXPlaybackStateID.Cooldown);
                    await UniTask.Delay((int)( SequenceTiming.CooldownDuration * 1000 ),
                        SequenceTiming.TimeScaleIndependent ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime,
                        cancellationToken: token);
                }
                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[FX] {this} was cancelled.");
                throw;
            }
            finally
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;
            }
        }
        private async UniTask PlayParallel(CancellationToken cancellationToken)
        {
            if (components != null)
            {
                List<UniTask> tasks = new List<UniTask>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    UniTask task = components[i].Play();
                    if (components[i].Timing.ContributeToTotalDuration)
                    {
                        tasks.Add(task);
                    }
                }
                await UniTask.WhenAll(tasks);
                OnCompleted?.Invoke();
            }
        }

        private async UniTask PlaySequential(CancellationToken cancellationToken)
        {
            if (components != null)
            {
                List<UniTask> tasks = new List<UniTask>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    UniTask task = components[i].Play().AttachExternalCancellation(cancellationToken);
                    await task;
                }
                OnCompleted?.Invoke();
            }
        }
        [Button]
        public void Stop()
        {
            if (!SetState(FXPlaybackStateID.Cancelled) || components == null)
            {
                return;
            }

            for (int i = 0; i < components.Length; i++)
            {
                components[i].Stop();
            }
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            SetState(FXPlaybackStateID.Idle);
            OnCancelled?.Invoke();
        }
        [Button]
        public void Reset()
        {
            if (!SetState(FXPlaybackStateID.Cancelled) || components == null)
            {
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Reset();
            }
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            SetState(FXPlaybackStateID.Idle);
        }
        private bool SetState(FXPlaybackStateID newState)
        {
            return stateMachine.TryTransitionTo(newState);
        }
    }
    public enum SequencePlayMode
    {
        Parallel = 0,
        Sequential = 1
    }
}

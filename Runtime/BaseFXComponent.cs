using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public abstract class BaseFXComponent : IFXComponent
    {
        FXTiming IFXComponent.Timing => Timing;
        [SerializeField] bool canPlayWhenAlreadyPlaying = true;
        [SerializeField] bool reverseOnCancel = false;
        [SerializeField][ShowIf("reverseOnCancel")] float reverseSpeedMultiplier = 1f;
        public FXTiming Timing;
        private FXPlaybackStateMachine stateMachine = new FXPlaybackStateMachine();
        public FXPlaybackStateID CurrentState => stateMachine.CurrentState;
        CancellationTokenSource cts;

        bool IFXComponent.CanPlayWhenAlreadyPlaying => canPlayWhenAlreadyPlaying;
        bool IFXComponent.ReverseOnCancel => reverseOnCancel;
        float IFXComponent.ReverseSpeedMultiplier => reverseSpeedMultiplier;

        public virtual void Initialize()
        {

        }
        [Button]
        public async UniTask Play()
        {
            await Play(1f);
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
            float effectiveSpeed = Timing.PlaybackSpeed * inheritedSpeed;
            float effectiveSpeedAbs = Mathf.Abs(effectiveSpeed);
            try
            {
                Timing.PlayCount = 0;
                SetState(FXPlaybackStateID.WaitingToStart);
                float calculatedInitialDelay = Timing.InitialDelay / effectiveSpeedAbs;
                float calculatedDelayBetweenRepeats = Timing.DelayBetweenRepeats / effectiveSpeedAbs;
                float calculatedCooldownDuration = Timing.CooldownDuration > 0 ? Timing.CooldownDuration / effectiveSpeedAbs : 0;
                if (effectiveSpeed > 0)
                {
                    await UniTask.Delay((int)( calculatedInitialDelay * 1000 ), cancellationToken: token); 
                }

                int repeatCount = Timing.RepeatForever ? int.MaxValue : Timing.NumberOfRepeats;
                for (int i = 0; i < repeatCount; i++)
                {
                    SetState(FXPlaybackStateID.Playing);
                    Timing.PlayCount++;
                    await PlayInternal(token, effectiveSpeed);

                    if (i < repeatCount - 1)
                    {
                        SetState(FXPlaybackStateID.RepeatingDelay);
                        await UniTask.Delay((int)( calculatedDelayBetweenRepeats * 1000 ), cancellationToken: token);
                    }
                }
                SetState(FXPlaybackStateID.Completed);
                if (calculatedCooldownDuration > 0)
                {
                    SetState(FXPlaybackStateID.Cooldown);
                    await UniTask.Delay((int)( calculatedCooldownDuration * 1000 ), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
                }
                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                if (stateMachine.CurrentState != FXPlaybackStateID.Cancelled || !reverseOnCancel)
                    throw;

                SetState(FXPlaybackStateID.Rewinding);
                Timing.PlayCount--;

                float effectiveReverseSpeed = inheritedSpeed * reverseSpeedMultiplier;
                await Reverse(effectiveReverseSpeed);
                float calculatedDelayBetweenRepeats = Timing.DelayBetweenRepeats / Mathf.Abs(effectiveReverseSpeed);
                while (Timing.PlayCount > 0)
                {
                    SetState(FXPlaybackStateID.RepeatingDelay);
                    await UniTask.Delay((int)( calculatedDelayBetweenRepeats * 1000 ),
                        Timing.TimeScaleIndependent ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime);
                    SetState(FXPlaybackStateID.Playing);
                    await Play(effectiveReverseSpeed);
                    Timing.PlayCount--;
                }
                SetState(FXPlaybackStateID.Completed);
                SetState(FXPlaybackStateID.Idle);
            }
            finally
            {
                CancellationTokenCleanUp();
            }
        }
        [Button]
        public void Stop()
        {
            if (!SetState(FXPlaybackStateID.Stopped))
            {
                return;
            }
            CancellationTokenCleanUp();
            StopInternal();
            SetState(FXPlaybackStateID.Idle);
        }
        protected virtual void StopInternal()
        {

        }
        [Button]
        public void Reset()
        {
            CancellationTokenCleanUp();
            ResetInternal();
            stateMachine.ForceSet(FXPlaybackStateID.Idle);
        }
        protected virtual void ResetInternal()
        {
        }

        protected virtual UniTask PlayInternal(CancellationToken cancellationToken, float effectiveSpeed = 1f)
        {
            return UniTask.CompletedTask;
        }

        private void CancellationTokenCleanUp()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        private bool SetState(FXPlaybackStateID newState)
        {
            return stateMachine.TryTransitionTo(newState);
        }
        [Button]
        public void Cancel()
        {
            if (!reverseOnCancel || stateMachine.CurrentState != FXPlaybackStateID.Playing)
            {
                Stop();
                return;
            }
            SetState(FXPlaybackStateID.Cancelled);
            cts?.Cancel();
        }
        protected virtual UniTask Reverse(float inheritedSpeed = 1f)
        {
            Stop();
            return UniTask.CompletedTask;
        }
    }
}
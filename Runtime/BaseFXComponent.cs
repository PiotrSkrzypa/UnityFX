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
        [SerializeField] bool canPlayWhenAlreadyPlaying = true;
        [SerializeField] bool rewindOnCancel = false;
        [SerializeField][ShowIf("reverseOnCancel")] float reverseSpeedMultiplier = 1f;
        FXTiming IFXComponent.Timing => Timing;
        public FXTiming Timing;
        public FXPlaybackStateID CurrentState => stateMachine.CurrentState;
        
        FXPlaybackStateMachine stateMachine = new FXPlaybackStateMachine();
        CancellationTokenSource cts;

        bool IFXComponent.CanPlayWhenAlreadyPlaying => canPlayWhenAlreadyPlaying;
        bool IFXComponent.ReverseOnCancel => rewindOnCancel;
        float IFXComponent.ReverseSpeedMultiplier => reverseSpeedMultiplier;

        public virtual void Initialize()
        {

        }
        [Button]
        public async UniTask Play() => await Play(1f);

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
            float effectiveReverseSpeed = inheritedSpeed * reverseSpeedMultiplier;
            try
            {
                Timing.PlayCount = 0;
                SetState(FXPlaybackStateID.WaitingToStart);

                if (effectiveSpeed > 0)
                {
                    await DelaySeconds(Timing.InitialDelay, effectiveSpeed, token, Timing.TimeScaleIndependent);
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
                        await DelaySeconds(Timing.DelayBetweenRepeats, effectiveSpeed, token, Timing.TimeScaleIndependent);
                    }
                }
                SetState(FXPlaybackStateID.Completed);

                SetState(FXPlaybackStateID.Cooldown);
                await DelaySeconds(Timing.CooldownDuration, effectiveSpeed, token, Timing.TimeScaleIndependent);

                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                if (stateMachine.CurrentState != FXPlaybackStateID.Cancelled || !rewindOnCancel)
                    throw;

                SetState(FXPlaybackStateID.Rewinding);
                Timing.PlayCount--;

                await Rewind(effectiveReverseSpeed);
                while (Timing.PlayCount > 0)
                {
                    SetState(FXPlaybackStateID.RepeatingDelay);
                    await DelaySeconds(Timing.DelayBetweenRepeats, effectiveReverseSpeed, token, Timing.TimeScaleIndependent);
                   
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
        private async UniTask DelaySeconds(float duration, float speed, CancellationToken token, bool unscaled = false)
        {
            if (duration <= 0f) return;

            var milliseconds = (int)((duration / speed) * 1000);
            var type = unscaled ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
            await UniTask.Delay(milliseconds, type, PlayerLoopTiming.Update, token);
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
            if (!rewindOnCancel || stateMachine.CurrentState != FXPlaybackStateID.Playing)
            {
                Stop();
                return;
            }
            SetState(FXPlaybackStateID.Cancelled);
            cts?.Cancel();
        }
        protected virtual UniTask Rewind(float inheritedSpeed = 1f)
        {
            Stop();
            return UniTask.CompletedTask;
        }
    }
}
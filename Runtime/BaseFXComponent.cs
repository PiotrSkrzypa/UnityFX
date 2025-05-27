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
        [SerializeField] bool CanPlayWhenAlreadyPlaying = true;
        public FXTiming Timing;
        private FXPlaybackStateMachine stateMachine = new FXPlaybackStateMachine();
        CancellationTokenSource cts;


        public virtual void Initialize()
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
            try
            {
                SetState(FXPlaybackStateID.WaitingToStart);
                await UniTask.Delay((int)( Timing.InitialDelay * 1000 ), cancellationToken: token);

                int repeatCount = Timing.RepeatForever ? int.MaxValue : Timing.NumberOfRepeats;
                for (int i = 0; i < repeatCount; i++)
                {
                    SetState(FXPlaybackStateID.Playing);
                    await PlayInternal(token);
                    Timing.PlayCount++;

                    if (i < repeatCount - 1)
                    {
                        SetState(FXPlaybackStateID.RepeatingDelay);
                        await UniTask.Delay((int)( Timing.DelayBetweenRepeats * 1000 ), cancellationToken: token);
                    }
                }
                SetState(FXPlaybackStateID.Completed);
                if (Timing.CooldownDuration > 0)
                {
                    SetState(FXPlaybackStateID.Cooldown);
                    await UniTask.Delay((int)( Timing.CooldownDuration * 1000 ), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
                }
                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Task was canceled");
            }
            finally
            {
                CancellationTokenCleanUp();
            }
        }
        [Button]
        public void Stop()
        {
            if (!SetState(FXPlaybackStateID.Cancelled))
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

        protected virtual UniTask PlayInternal(CancellationToken cancellationToken)
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

    }
}
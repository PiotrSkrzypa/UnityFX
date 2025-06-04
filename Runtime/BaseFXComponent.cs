using System;
using System.Threading;
using Alchemy.Inspector;
using Codice.CM.Common;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.GraphicsBuffer;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public abstract class BaseFXComponent : IFXComponent
    {
        [SerializeField] bool canPlayWhenAlreadyPlaying = true;
        [SerializeField] bool rewindOnCancel = false;
        [SerializeField][Alchemy.Inspector.DisableAlchemyEditor] PlaybackSpeed playbackSpeedSettings = new PlaybackSpeed { speed = 1f, rewindSpeed = -1f };
        [SerializeField]Ease easeType = Ease.Linear;
        FXTiming IFXComponent.Timing => Timing;
        public FXTiming Timing;
        public FXPlaybackStateID CurrentState => stateMachine.CurrentState;

        FXPlaybackStateMachine stateMachine = new FXPlaybackStateMachine();
        CancellationTokenSource cts;

        bool IFXComponent.CanPlayWhenAlreadyPlaying => canPlayWhenAlreadyPlaying;
        bool IFXComponent.RewindOnCancel => rewindOnCancel;
        float IFXComponent.RewindSpeedMultiplier => playbackSpeedSettings.rewindSpeed;

        public float Progress => progress;
        protected float progress;


        public virtual void Initialize()
        {

        }

        [Button]
        public async UniTask Play() => await Play(new PlaybackSpeed(1f, 1f));

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

        [Button]
        public void Reset()
        {
            CancellationTokenCleanUp();
            ResetInternal();
            stateMachine.ForceSet(FXPlaybackStateID.Idle);
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


        public async UniTask Play(PlaybackSpeed inheritedPlaybackSpeed)
        {
            if (stateMachine.CurrentState != FXPlaybackStateID.Idle)
            {
                if (canPlayWhenAlreadyPlaying)
                {
                    Stop();
                }
                else
                {
                    Debug.LogWarning($"[FX] Cannot play {this} when already playing.");
                    return;
                }
            }
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            PlaybackSpeed combinedPlaybackSpeed = playbackSpeedSettings * inheritedPlaybackSpeed;

            try
            {
                Timing.PlayCount = 0;
                SetState(FXPlaybackStateID.WaitingToStart);

                await DelaySeconds(Timing.InitialDelay, combinedPlaybackSpeed.SpeedAbs, token, Timing.TimeScaleIndependent);

                await RunLoops(combinedPlaybackSpeed, token);

                SetState(FXPlaybackStateID.Completed);

                SetState(FXPlaybackStateID.Cooldown);
                await DelaySeconds(Timing.CooldownDuration, combinedPlaybackSpeed.SpeedAbs, token, Timing.TimeScaleIndependent);

                SetState(FXPlaybackStateID.Idle);
            }
            catch (OperationCanceledException)
            {
                if (stateMachine.CurrentState != FXPlaybackStateID.Cancelled || !rewindOnCancel)
                    throw;

                await HandleCanellation(combinedPlaybackSpeed, token);
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
            var type = unscaled ? Cysharp.Threading.Tasks.DelayType.UnscaledDeltaTime : Cysharp.Threading.Tasks.DelayType.DeltaTime;
            await UniTask.Delay(milliseconds, type, PlayerLoopTiming.Update, token);
        }

        private async UniTask RunLoops(PlaybackSpeed combinedPlaybackSpeed, CancellationToken token)
        {
            int repeatCount = Timing.RepeatForever ? int.MaxValue : Timing.NumberOfRepeats;
            for (int i = 0; i < repeatCount; i++)
            {
                SetState(FXPlaybackStateID.Playing);
                Timing.PlayCount++;
                await PlayInternal(token, combinedPlaybackSpeed);

                if (i < repeatCount - 1)
                {
                    SetState(FXPlaybackStateID.RepeatingDelay);
                    await DelaySeconds(Timing.DelayBetweenRepeats, combinedPlaybackSpeed.SpeedAbs, token, Timing.TimeScaleIndependent);
                }
            }
        }
        private async UniTask HandleCanellation(PlaybackSpeed combinedPlaybackSpeed, CancellationToken token)
        {
            SetState(FXPlaybackStateID.Rewinding);
            Timing.PlayCount--;

            await Rewind(combinedPlaybackSpeed);
            while (Timing.PlayCount > 0)
            {
                SetState(FXPlaybackStateID.RepeatingDelay);
                await DelaySeconds(Timing.DelayBetweenRepeats, combinedPlaybackSpeed.RewindSpeedAbs, token, Timing.TimeScaleIndependent);

                SetState(FXPlaybackStateID.Playing);
                await Play(new PlaybackSpeed(combinedPlaybackSpeed.rewindSpeed, combinedPlaybackSpeed.rewindSpeed));
                Timing.PlayCount--;
            }
            SetState(FXPlaybackStateID.Completed);
            SetState(FXPlaybackStateID.Idle);
        }


        
        protected virtual void StopInternal()
        {

        }
       
        protected virtual void ResetInternal()
        {
        }

        protected async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            var scheduler = Timing.GetScheduler();
            float duration = GetCalculatedDuration(playbackSpeed);

            float from = playbackSpeed.speed > 0 ? 0f : 1f;
            float to = playbackSpeed.speed > 0 ? 1f : 0f;
            Update(from);
            try
            {
                var motion = LMotion.Create(from, to, duration)
            .WithEase(easeType)
            .WithScheduler(scheduler)
            .Bind(this, static (x, self) => {self.progress = x; self.Update(x); });

                await motion.ToUniTask(cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
            Update(to);
        }

        protected async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            var scheduler = Timing.GetScheduler();
            float duration = GetCalculatedRewindDuration(playbackSpeed);

            float from = progress;
            float to = 0f;
            Update(from);
            var motion = LMotion.Create(from, to, duration)
            .WithEase(easeType)
            .WithScheduler(scheduler)
            .Bind(this, static (x, self) => {self.progress = x; self.Update(x); });

            await motion.ToUniTask();
            Update(to);
        }

        protected abstract void Update(float progress);


        protected float GetCalculatedDuration(PlaybackSpeed playbackSpeed)
        {
            return Timing.Duration / playbackSpeed.SpeedAbs;
        }

        protected float GetCalculatedRewindDuration(PlaybackSpeed playbackSpeed)
        {
            return Timing.Duration * progress / playbackSpeed.RewindSpeedAbs;
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
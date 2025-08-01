using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using DelayType = LitMotion.DelayType;

namespace PSkrzypa.UnityFX.V2
{
    public abstract class FXAnimationComponent : FXComponent
    {
        private const float REWIND_EPSILON = 0.001f;
        [SerializeField] FXAnimationSettings settings = new FXAnimationSettings();

        float lastPlaybackSpeed = 1f;
        CancellationTokenSource cts;

        public sealed override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1f)
        {
            if(!isInitialized)
            {
                Initialize();
                isInitialized = true;
            }
            if (trackedHandle.IsActive())
            {
                PlayExistingMotion(playbackDirection, playbackSpeedModifier);
                return;
            }
            CreateNewMotion(playbackDirection, playbackSpeedModifier);
            isPlaying = true;
        }

        private void PlayExistingMotion(PlaybackDirection playbackDirection, float playbackSpeedModifier)
        {
            switch (playbackDirection)
            {
                case PlaybackDirection.Forward:
                    trackedHandle.PlaybackSpeed = settings.ForwardPlaybackSpeed * playbackSpeedModifier;
                    trackedHandle.Time = isPlaying ? trackedHandle.Time : 0f;
                    break;
                case PlaybackDirection.Backward:
                    trackedHandle.PlaybackSpeed = -settings.BackwardPlaybackSpeed * playbackSpeedModifier;
                    trackedHandle.Time = isPlaying ? trackedHandle.Time : trackedHandle.TotalDuration - REWIND_EPSILON;
                    WaitUntilRewindComplete().Forget();
                    break;
            }
        }

        private void CreateNewMotion(PlaybackDirection playbackDirection, float playbackSpeedModifier)
        {
            float playbackSpeed = playbackDirection == PlaybackDirection.Forward ? settings.ForwardPlaybackSpeed : -settings.BackwardPlaybackSpeed;
            playbackSpeed *= playbackSpeedModifier;

            var motionBuilder = LMotion.Create(0f, 1f, settings.Duration)
                .WithDelay(settings.Delay, settings.DelayType)
                .WithLoops(settings.Loops, settings.LoopType)
                .WithEase(settings.Ease)
                .WithScheduler(settings.TimescaleIndependent?MotionScheduler.UpdateRealtime:MotionScheduler.Update)
                .WithOnLoopComplete(OnLoopComplete)
                .WithOnComplete(OnComplete)
                .WithOnCancel(OnCancel);

            MotionHandle handle = motionBuilder.Bind(this, static(x, self) =>
            {
                self.Progress = x;
                self.OnUpdate(x);
            });

            handle.PlaybackSpeed = playbackSpeed;
            handle.Time = playbackDirection == PlaybackDirection.Forward ? 0f : handle.TotalDuration - REWIND_EPSILON;
            TrackedHandle = handle;
            if (playbackDirection == PlaybackDirection.Backward)
            {
                WaitUntilRewindComplete().Forget();
            }
            TrackedHandle.Preserve();
        }
        void OnLoopComplete(int loopCount)
        {
            
        }
        void OnComplete()
        {
            trackedHandle.Time = trackedHandle.TotalDuration;
            trackedHandle.TryCancel();
            isPlaying = false;
            callbackActions.OnCompleteAction.Invoke();
        }
        void OnCancel()
        {
            callbackActions.OnCancelAction.Invoke();
        }
        async UniTaskVoid WaitUntilRewindComplete()
        {
            CancellationTokenCleanUp();
            cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;
            if (TrackedHandle.IsActive())
            {
                PlayerLoopTiming playerLoopTiming = settings.TimescaleIndependent ? PlayerLoopTiming.TimeUpdate : PlayerLoopTiming.Update;
                await UniTask.WaitUntil(() => TrackedHandle.Time < 0, playerLoopTiming, cancellationToken);
                OnRewindComplete();
            }
        }
        void CancellationTokenCleanUp()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
        void OnRewindComplete()
        {
            trackedHandle.Time = 0f;
            trackedHandle.TryCancel();
            isPlaying = false;
            callbackActions.OnRewindCompleteAction.Invoke();
        }

        public override void Rewind(float playbackSpeedModifier = 1f)
        {
            Play(PlaybackDirection.Backward, playbackSpeedModifier);
        }

        public sealed override void Pause()
        {
            if (trackedHandle.IsActive() && trackedHandle.IsPlaying())
            {
                lastPlaybackSpeed = trackedHandle.PlaybackSpeed;
                trackedHandle.PlaybackSpeed = 0f;
            }
        }
        public sealed override void Resume()
        {
            if (trackedHandle.IsActive() && trackedHandle.IsPlaying())
            {
                trackedHandle.PlaybackSpeed = lastPlaybackSpeed;
            }
        }
        public sealed override void Stop()
        {
            Reset();
            CancellationTokenCleanUp();
            TrackedHandle.TryCancel();
            Progress = 0f;
            isPlaying = false;
        }
        protected virtual void Reset() { }
    }
}
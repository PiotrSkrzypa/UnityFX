using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Particles/Enable Trail Renderer")]
    public class FXEnableTrailRenderer : FXComponent
    {
        [SerializeField] TrailRenderer trailRenderer;
        [SerializeField] bool targetState = true;
        bool originalState;

        protected override void Initialize()
        {
            originalState = trailRenderer.emitting;
        }

        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
        {
            trailRenderer.emitting = playbackDirection == PlaybackDirection.Forward ? targetState : originalState;
            callbackActions.OnCompleteAction?.Invoke();
        }

        public override void Rewind(float playbackSpeedModifier = 1)
        {
            trailRenderer.emitting = originalState;
            callbackActions.OnRewindCompleteAction?.Invoke();
        }
    }
}

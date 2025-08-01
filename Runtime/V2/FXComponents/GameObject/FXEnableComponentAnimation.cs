using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Game Object/Enable Component")]
    public class FXEnableComponentAnimation : FXComponent
    {
        [SerializeField] Behaviour componentToEnable;
        [SerializeField] bool targetState = true;

        bool originalState;

        protected override void Initialize()
        {
            if (componentToEnable == null)
            {
                Debug.LogWarning("[EnableComponentTweenAnimation] componentToEnable is null.");
                return;
            }
            originalState = componentToEnable.enabled;
        }
        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
        {
            componentToEnable.enabled = playbackDirection == PlaybackDirection.Forward ? targetState : originalState;
            callbackActions.OnCompleteAction?.Invoke();
        }

        public override void Rewind(float playbackSpeedModifier = 1)
        {
            componentToEnable.enabled = originalState;
            callbackActions.OnRewindCompleteAction?.Invoke();
        }

    }
}

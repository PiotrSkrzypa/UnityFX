using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Audio/Play Sound")]
    public sealed class FXPlaySound : FXComponent
    {
        [SerializeField] AudioClip audioClip;
        [SerializeField] string uiAudioClipKey;
        [SerializeField] bool isUISound;

        protected override void Initialize()
        {
            if (audioClip == null)
            {
                Debug.LogWarning("[FXAudioEffect] audio clip is null.");
                return;
            }
        }
        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
        {
            if (playbackDirection == PlaybackDirection.Forward)
            {
                AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
            }
            callbackActions.OnCompleteAction?.Invoke();
        }

        public override void Rewind(float playbackSpeedModifier = 1)
        {
            callbackActions.OnRewindCompleteAction?.Invoke();
        }
    }
}
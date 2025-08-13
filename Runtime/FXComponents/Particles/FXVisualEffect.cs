using System;
using UnityEngine;
using UnityEngine.VFX;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Particles/Visual Effect")]
    public class FXVisualEffect : FXComponent
    {
        [SerializeField] VisualEffect visualEffect;

        protected override void Initialize()
        {
            base.Initialize();
            visualEffect.Stop();
        }
        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
        {
            visualEffect.pause = false;
            visualEffect.playRate = playbackDirection == PlaybackDirection.Forward ? playbackSpeedModifier : -playbackSpeedModifier;
            visualEffect.Play();
        }
        public override void Rewind(float playbackSpeedModifier = 1)
        {
            visualEffect.pause = false;
            visualEffect.playRate = -playbackSpeedModifier;
            visualEffect.Play();
        }
        public override void Pause()
        {
            visualEffect.pause = true;
        }
        public override void Resume()
        {
            visualEffect.pause = false;
        }
        public override void Stop()
        {
            visualEffect.Stop();
        }

    }
}
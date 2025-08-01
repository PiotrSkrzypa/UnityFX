using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Particles/Particle System")]
    public class FXParticleSystem : FXComponent
    {
        [SerializeField] ParticleSystem particleSystem;

        // TODO Invoke callbacks when particle system is finished

        protected override void Initialize()
        {
            particleSystem.Stop();
        }
        public override void Stop()
        {
            particleSystem.Stop();
        }

        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
        {
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.simulationSpeed = playbackDirection ==PlaybackDirection.Forward ? playbackSpeedModifier : -playbackSpeedModifier;
            particleSystem.Play();
            callbackActions.OnCompleteAction?.Invoke();
        }
        public override void Rewind(float playbackSpeedModifier = 1)
        {
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.simulationSpeed = -playbackSpeedModifier;
            particleSystem.Play();
            callbackActions.OnRewindCompleteAction?.Invoke();
        }
        public override void Pause()
        {
            particleSystem.Pause();
        }
        public override void Resume()
        {
            particleSystem.Play();
        }
    }
}

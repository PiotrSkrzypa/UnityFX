using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXParticleSystem : BaseFXComponent
    {
        [SerializeField] ParticleSystem particleSystem;

        public override void Initialize()
        {
            particleSystem.Stop();
        }
        protected override void StopInternal()
        {
            particleSystem.Stop();
        }
        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            await UniTask.Yield();
            particleSystem.time = 0;
            particleSystem.Play();
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            await UniTask.Yield();
            particleSystem.time = 0;
            particleSystem.Play();
        }
    }
}

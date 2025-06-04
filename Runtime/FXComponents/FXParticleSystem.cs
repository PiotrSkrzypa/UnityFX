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

        protected override void Update(float progress)
        {
            if(progress == 0)
            {
                particleSystem.time = 0;
                particleSystem.Play();
            }
        }

        public override void Initialize()
        {
            particleSystem.Stop();
        }
        protected override void StopInternal()
        {
            particleSystem.Stop();
        }
        protected override void ResetInternal()
        {
            particleSystem.Stop();
        }
    }
}

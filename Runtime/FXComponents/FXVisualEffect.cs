using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXVisualEffect : BaseFXComponent
    {
        [SerializeField] VisualEffect visualEffect;

        protected override void Update(float progress)
        {
            if (progress == 0)
            {
                visualEffect.Reinit();
                visualEffect.Play();
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            visualEffect.Stop();
        }
        protected override void StopInternal()
        {
            visualEffect.Stop();
        }
        protected override void ResetInternal()
        {
            visualEffect.Stop();
        }
    }
}
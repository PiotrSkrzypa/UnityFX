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

        public override void Initialize()
        {
            base.Initialize();
            visualEffect.Stop();
        }
        protected override void StopInternal()
        {
            visualEffect.Stop();
        }
        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            visualEffect.Reinit();
            visualEffect.playRate = inheritedSpeed;
            visualEffect.Play();
            await UniTask.CompletedTask;
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1)
        {
            visualEffect.Reinit();
            visualEffect.playRate = inheritedSpeed;
            visualEffect.Play();
            await UniTask.CompletedTask;
        }
    }
}
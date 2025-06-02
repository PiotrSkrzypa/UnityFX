using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXFXObject : BaseFXComponent
    {
        [SerializeField] FXPlayer fxObject;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            await fxObject.Play(cancellationToken, inheritedSpeed);
        }
        protected override void StopInternal()
        {
            fxObject.Stop();
        }
        protected override void ResetInternal()
        {
            fxObject.ResetComponents();
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1)
        {
            fxObject.Cancel();
            await UniTask.WaitWhile(() => fxObject.IsPlaying);
        }
    }
}
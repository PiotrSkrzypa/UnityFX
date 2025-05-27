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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            await fxObject.Play(cancellationToken);
        }
    }
}
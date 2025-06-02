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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            await fxObject.Play(cancellationToken, playbackSpeed);
        }
        protected override void StopInternal()
        {
            fxObject.Stop();
        }
        protected override void ResetInternal()
        {
            fxObject.ResetComponents();
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            fxObject.Cancel();
            await UniTask.WaitWhile(() => fxObject.IsPlaying);
        }
    }
}
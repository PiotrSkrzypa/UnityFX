using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public sealed class FXAudioEffect : BaseFXComponent
    {
        [SerializeField] AudioClip audioClip;
        [SerializeField] string uiAudioClipKey;
        [SerializeField] bool isUISound;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
            await UniTask.CompletedTask;
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1f)
        {
            // Audio effects typically do not reverse, so this can be left empty or throw an exception if needed.
            await UniTask.CompletedTask;
        }
    }
}
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

        protected override void Update(float progress)
        {
            if(progress == 1f)
            {
                AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
            }
        }
    }
}
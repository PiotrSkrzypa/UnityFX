using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PostProcessVolumeTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Volume volume;
        [SerializeField] private float startWeight = 0f;
        [SerializeField] private float targetWeight = 1f;

        protected override void Update(float progress)
        {
            volume.weight = Mathf.LerpUnclamped(startWeight, targetWeight, progress);
        }

        protected override void StopInternal()
        {
            if (volume != null)
                volume.weight = startWeight;
        }

        protected override void ResetInternal()
        {
            if (volume != null)
                volume.weight = startWeight;
        }

    }
}

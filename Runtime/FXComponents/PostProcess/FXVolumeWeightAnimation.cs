using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Post Process/Volume Weight")]
    public class FXVolumeWeightAnimation : FXAnimationComponent
    {
        [SerializeField] private Volume volume;
        [SerializeField] private float startWeight = 0f;
        [SerializeField] private float targetWeight = 1f;

        protected override void OnUpdate(float progress)
        {
            volume.weight = Mathf.LerpUnclamped(startWeight, targetWeight, progress);
        }

        protected override void Reset()
        {
            if (volume != null)
                volume.weight = startWeight;
        }

    }
}

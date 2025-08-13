using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Transform/Scale")]
    [FXComponentIcon("d_Transform Icon")]
    public sealed class FXScaleAnimation : FXAnimationComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 targetScale;
        Vector3 originalScale;

        protected override void Initialize()
        {
            originalScale = transformToScale.localScale;
        }
        protected override void OnUpdate(float progress)
        {
            transformToScale.localScale = Vector3.LerpUnclamped(originalScale, targetScale, progress);
        }

        protected override void Reset()
        {
            if (isInitialized)
            {
                transformToScale.localScale = originalScale;
                isInitialized = false; 
            }
        }
    }
}

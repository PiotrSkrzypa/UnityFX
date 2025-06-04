using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 targetScale;
        Vector3 originalScale;


        public override void Initialize()
        {
            originalScale = transformToScale.localScale;
        }

        protected override void Update(float progress)
        {
            transformToScale.localScale = Vector3.LerpUnclamped(originalScale, targetScale, progress);
        }
        
        protected override void StopInternal()
        {
            transformToScale.localScale = originalScale;
        }

        protected override void ResetInternal()
        {
            transformToScale.localScale = originalScale;
        }
    }
}

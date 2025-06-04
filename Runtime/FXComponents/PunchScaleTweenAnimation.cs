using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 punch = Vector3.one * 0.5f;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;

        private Vector3 originalScale;

        public override void Initialize()
        {
            originalScale = transformToScale.localScale;
        }
        protected override void Update(float progress)
        {

            if (progress == 1f || progress == 0f)
            {
                transformToScale.localScale = originalScale;
                return;
            }
            float angularFrequency = (frequency - 0.5f) * Mathf.PI;
            float dampingFactor = damping * frequency / (2f * Mathf.PI);
            Vector3 offset = Mathf.Cos(angularFrequency * progress) * Mathf.Pow(Mathf.Epsilon, -dampingFactor * progress) * punch;
            transformToScale.localScale = originalScale + offset;
        }
        protected override void StopInternal()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }

        protected override void ResetInternal()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }
    }
}

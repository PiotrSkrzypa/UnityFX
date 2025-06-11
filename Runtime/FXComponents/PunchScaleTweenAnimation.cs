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
        private const float PI = Mathf.PI;
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
            float t = (progress - 0.5f) * 2f;
            float angularFrequency = PI * frequency;
            float dampingFactor = damping * frequency / (2f * PI);

            float oscillation = Mathf.Sin(progress * angularFrequency);
            float decay = Mathf.Exp(-dampingFactor * Mathf.Abs(t));

            Vector3 offset = oscillation * decay * punch;
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

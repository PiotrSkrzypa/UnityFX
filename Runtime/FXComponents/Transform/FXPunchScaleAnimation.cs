using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Transform/Punch Scale")]
    [FXComponentIcon("d_Transform Icon")]
    public class FXPunchScaleAnimation : FXAnimationComponent
    {
        private const float PI = Mathf.PI;
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 punch = Vector3.one * 0.5f;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;

        private Vector3 originalScale;

        protected override void Initialize()
        {
            originalScale = transformToScale.localScale;
        }
        protected override void OnUpdate(float progress)
        {
            float t = (progress - 0.5f) * 2f;
            float angularFrequency = PI * frequency;
            float dampingFactor = damping * frequency / (2f * PI);

            float oscillation = Mathf.Sin(progress * angularFrequency);
            float decay = Mathf.Exp(-dampingFactor * Mathf.Abs(t));

            Vector3 offset = oscillation * decay * punch;
            transformToScale.localScale = originalScale + offset;
        }
        protected override void Reset()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }
    }
}

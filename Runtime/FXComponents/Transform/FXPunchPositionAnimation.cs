using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Transform/Punch Position")]
    [FXComponentIcon("d_Transform Icon")]
    public class FXPunchPositionAnimation : FXAnimationComponent
    {
        private const float PI = Mathf.PI;
        public int Frequency { get => frequency; set => frequency = value; }
        public float Damping { get => damping; set => damping = value; }

        [SerializeField] private Transform transformToMove;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;
        [SerializeField] private Vector3 punch;

        Vector3 originalPosition;

        protected override void Initialize()
        {
            if (useLocalSpace)
            {
                originalPosition = transformToMove.localPosition;
            }
            else
            {
                originalPosition = transformToMove.position;
            }
        }

        protected override void OnUpdate(float progress)
        {
            float t = (progress - 0.5f) * 2f;
            float angularFrequency = PI * frequency;
            float dampingFactor = damping * frequency / (2f * PI);

            float oscillation = Mathf.Sin(progress * angularFrequency);
            float decay = Mathf.Exp(-dampingFactor * Mathf.Abs(t));

            Vector3 offset = oscillation * decay * punch;
            if (useLocalSpace)
            {
                transformToMove.localPosition = originalPosition + offset;
            }
            else
            {
                transformToMove.position = originalPosition + offset;
            }
        }
        
        protected override void Reset()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = originalPosition;
            }
            else
            {
                transformToMove.position = originalPosition;
            }
        }
    }
}

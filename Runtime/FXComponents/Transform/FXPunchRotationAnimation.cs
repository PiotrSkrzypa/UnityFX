using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Transform/Punch Rotation")]
    [FXComponentIcon("d_Transform Icon")]
    public class FXPunchRotationAnimation : FXAnimationComponent
    {
        private const float PI = Mathf.PI;
        public int Frequency { get => frequency; set => frequency = value; }
        public float Damping { get => damping; set => damping = value; }

        [SerializeField] private Transform transformToRotate;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;
        [SerializeField] private Vector3 punch;
        Vector3 originalRotation;

        protected override void Initialize()
        {
            if (useLocalSpace)
            {
                originalRotation = transformToRotate.localEulerAngles;
            }
            else
            {
                originalRotation = transformToRotate.eulerAngles;
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
                transformToRotate.localEulerAngles = originalRotation + offset;
            }
            else
            {
                transformToRotate.eulerAngles = originalRotation + offset;
            }
        }

        protected override void Reset()
        {
            if (useLocalSpace)
            {
                transformToRotate.localEulerAngles = originalRotation;
            }
            else
            {
                transformToRotate.eulerAngles = originalRotation;
            }
        }
    }
}

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;
using DG.Tweening;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchRotationTweenAnimation : BaseFXComponent
    {
        public int Frequency { get => frequency; set => frequency = value; }
        public float Damping { get => damping; set => damping = value; }

        [SerializeField] private Transform transformToRotate;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;
        [SerializeField] private Vector3 punch;
        Vector3 originalRotation;

        public override void Initialize()
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
        protected override void Update(float progress)
        {

            if (progress == 1f || progress == 0f)
            {
                if (useLocalSpace)
                {
                    transformToRotate.localEulerAngles = originalRotation;
                }
                else
                {
                    transformToRotate.eulerAngles = originalRotation;
                }
                return;
            }
            float angularFrequency = (frequency - 0.5f) * Mathf.PI;
            float dampingFactor = damping * frequency / (2f * Mathf.PI);
            Vector3 offset = Mathf.Cos(angularFrequency * progress) * Mathf.Pow(Mathf.Epsilon, -dampingFactor * progress) * punch;
            if (useLocalSpace)
            {
                transformToRotate.localEulerAngles = originalRotation + offset;
            }
            else
            {
                transformToRotate.eulerAngles = originalRotation + offset;
            }
        }

        protected override void StopInternal()
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
        protected override void ResetInternal()
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

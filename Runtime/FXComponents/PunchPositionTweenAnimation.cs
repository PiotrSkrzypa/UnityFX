using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchPositionTweenAnimation : BaseFXComponent
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

        public override void Initialize()
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

        protected override void Update(float progress)
        {

            if (progress == 1f || progress == 0f)
            {
                if(useLocalSpace)
                {
                    transformToMove.localPosition = originalPosition;
                }
                else
                {
                    transformToMove.position = originalPosition;
                }
                return;
            }
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
        
        protected override void StopInternal()
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
        protected override void ResetInternal()
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

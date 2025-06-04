using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PositionTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToMove;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] private Vector3 startingPosition;
        [SerializeField] private Vector3 targetPosition;

        protected override void Update(float progress)
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = Vector3.LerpUnclamped(startingPosition, targetPosition, progress);
            }
            else
            {
                transformToMove.position = Vector3.LerpUnclamped(startingPosition, targetPosition, progress);
            }
        }

        private void ResetPosition()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = startingPosition;
            }
            else
            {
                transformToMove.position = startingPosition;
            }
        }
        protected override void StopInternal()
        {
            ResetPosition();
        }
        protected override void ResetInternal()
        {
            ResetPosition();
        }

    }
}

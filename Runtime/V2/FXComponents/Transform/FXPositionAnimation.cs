using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Transform/Position")]
    [FXComponentIcon("d_Transform Icon")]
    public sealed class FXPositionAnimation : FXAnimationComponent
    {
        [SerializeField] private Transform transformToMove;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] private Vector3 startingPosition;
        [SerializeField] private Vector3 targetPosition;

        protected override void OnUpdate(float progress)
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

        protected override void Reset()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = startingPosition;
            }
            else
            {
                transformToMove.position = startingPosition;
            }
            isInitialized = false;
        }

    }
}

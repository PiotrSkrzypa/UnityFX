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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            if (transformToMove == null)
                return;

            Vector3 fromPosition = inheritedSpeed > 0 ? startingPosition : targetPosition;
            Vector3 toPosition = inheritedSpeed > 0 ? targetPosition : startingPosition;
            if (useLocalSpace)
            {
                transformToMove.localPosition = fromPosition;
            }
            else
            {
                transformToMove.position = fromPosition;
            }
            var scheduler = Timing.GetScheduler();
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var morionBuilder = LMotion.Create(fromPosition, toPosition, calculatedDuration)
                    .WithEase(Ease.OutQuad)
                    .WithScheduler(scheduler);
            var handle = useLocalSpace ? morionBuilder.Bind(transformToMove, (x, t) => t.localPosition = x) :
                    morionBuilder.Bind(transformToMove, (x, t) => t.position = x);

            await handle.ToUniTask(cancellationToken);
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1f)
        {
            if (transformToMove == null)
                return;

            Vector3 currentPosition = useLocalSpace ? transformToMove.localPosition : transformToMove.position;
            var scheduler = Timing.GetScheduler();
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var morionBuilder = LMotion.Create(currentPosition, startingPosition, calculatedDuration)
                    .WithEase(Ease.OutQuad)
                    .WithScheduler(scheduler);
            var handle = useLocalSpace ? morionBuilder.Bind(transformToMove, (x, t) => t.localPosition = x) :
                    morionBuilder.Bind(transformToMove, (x, t) => t.position = x);

            await handle.ToUniTask();
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

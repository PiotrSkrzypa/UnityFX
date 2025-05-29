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
        public int Frequency { get => frequency; set => frequency = value; }
        public float Damping { get => damping; set => damping = value; }

        [SerializeField] private Transform transformToMove;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private Vector3 punch;

        Vector3 originalPosition;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            var scheduler = Timing.GetScheduler();

            originalPosition = useLocalSpace ? transformToMove.localPosition : transformToMove.position;
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var motionBuilder = LMotion.Punch.Create(originalPosition, punch, calculatedDuration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithEase(ease)
                .WithScheduler(scheduler);
            UniTask uniTask = useLocalSpace ? 
                motionBuilder.Bind(transformToMove, (v, tr) => transformToMove.localPosition = v)
                .ToUniTask(cancellationToken) :
                motionBuilder.Bind(transformToMove, (v, tr) => transformToMove.position = v)
                .ToUniTask(cancellationToken);

            await uniTask;
        }
        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            var scheduler = Timing.GetScheduler();

            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var motionBuilder = LMotion.Punch.Create(originalPosition, punch, calculatedDuration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithEase(ease)
                .WithScheduler(scheduler);
            UniTask uniTask = useLocalSpace ?
                motionBuilder.Bind(transformToMove, (v, tr) => transformToMove.localPosition = v)
                .ToUniTask() :
                motionBuilder.Bind(transformToMove, (v, tr) => transformToMove.position = v)
                .ToUniTask();

            await uniTask;
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

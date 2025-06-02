using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;

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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            var scheduler = Timing.GetScheduler();
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.speed);
            originalRotation = useLocalSpace ? transformToRotate.localEulerAngles : transformToRotate.eulerAngles;

            var motionBuilder = LMotion.Punch.Create(originalRotation, punch, calculatedDuration)
                            .WithFrequency(frequency)
                            .WithDampingRatio(damping)
                            .WithScheduler(scheduler);
            UniTask uniTask = useLocalSpace ?
                motionBuilder.Bind(transformToRotate, (v, tr) => transformToRotate.localEulerAngles = v)
                .ToUniTask(cancellationToken) :
               motionBuilder.Bind(transformToRotate, (v, tr) => transformToRotate.eulerAngles = v)
                .ToUniTask(cancellationToken);

            await uniTask;
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            var scheduler = Timing.GetScheduler();
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.rewindSpeed);
            var motionBuilder = LMotion.Punch.Create(originalRotation, punch, calculatedDuration)
                            .WithFrequency(frequency)
                            .WithDampingRatio(damping)
                            .WithScheduler(scheduler);
            UniTask uniTask = useLocalSpace ?
                motionBuilder.Bind(transformToRotate, (v, tr) => transformToRotate.localEulerAngles = v)
                .ToUniTask() :
               motionBuilder.Bind(transformToRotate, (v, tr) => transformToRotate.eulerAngles = v)
                .ToUniTask();

            await uniTask;
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

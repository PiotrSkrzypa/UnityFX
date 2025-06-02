using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 punch = Vector3.one * 0.5f;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;

        private Vector3 originalScale;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            originalScale = transformToScale.localScale;

            var scheduler = Timing.GetScheduler();

            var punchTween = LMotion.Punch.Create(originalScale, punch, calculatedDuration)
                    .WithFrequency(frequency)
                    .WithDampingRatio(damping)
                    .WithScheduler(scheduler)
                    .Bind(transformToScale, (v, t) => t.localScale = v);

            await punchTween.ToUniTask(cancellationToken);
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var scheduler = Timing.GetScheduler();

            var punchTween = LMotion.Punch.Create(originalScale, punch, calculatedDuration)
                    .WithFrequency(frequency)
                    .WithDampingRatio(damping)
                    .WithScheduler(scheduler)
                    .Bind(transformToScale, (v, t) => t.localScale = v);

            await punchTween.ToUniTask();
        }
        protected override void StopInternal()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }

        protected override void ResetInternal()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }
    }
}

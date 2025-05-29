using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 targetScale;
        Vector3 originalScale;

        public override void Initialize()
        {
            originalScale = transformToScale.localScale;
        }
        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            var scheduler = Timing.GetScheduler();

            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Vector3 fromScale = inheritedSpeed > 0 ? originalScale : targetScale;
            Vector3 toScale = inheritedSpeed > 0 ? targetScale : originalScale;
            UniTask uniTask = LMotion.Create(fromScale, toScale, calculatedDuration)
                .WithScheduler(scheduler)
                .Bind(transformToScale, (v, tr) => transformToScale.localScale = v)
                .ToUniTask(cancellationToken);

            await uniTask;
        }
        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            var scheduler = Timing.GetScheduler();

            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Vector3 currentScale = transformToScale.localScale;
            UniTask uniTask = LMotion.Create(currentScale, originalScale, calculatedDuration)
                .WithScheduler(scheduler)
                .Bind(transformToScale, (v, tr) => transformToScale.localScale = v)
                .ToUniTask();

            await uniTask;
        }
        protected override void StopInternal()
        {
            transformToScale.localScale = originalScale;
        }

        protected override void ResetInternal()
        {
            transformToScale.localScale = originalScale;
        }
    }
}

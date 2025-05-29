using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CanvasGroupAlphaTweenAnimation : BaseFXComponent
    {
        [SerializeField] CanvasGroup targetCanvasGroup;
        [SerializeField] float startAlpha;
        [SerializeField] float targetAlpha;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var handle = LMotion.Create(startAlpha, targetAlpha, calculatedDuration)
                .WithEase(LitMotion.Ease.OutQuad).WithScheduler(Timing.GetScheduler())
                .BindToAlpha(targetCanvasGroup);
            await handle.ToUniTask(cancellationToken);
        }
        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            float currentAlpha = targetCanvasGroup.alpha;
            var handle = LMotion.Create(currentAlpha, startAlpha, calculatedDuration)
                .WithEase(LitMotion.Ease.OutQuad).WithScheduler(Timing.GetScheduler())
                .BindToAlpha(targetCanvasGroup);
            await handle.ToUniTask();
        }
        protected override void StopInternal()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
        protected override void ResetInternal()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
    }
}

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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.speed);
            float fromAlpha = playbackSpeed.speed > 0 ? startAlpha : targetAlpha;
            float toAlpha = playbackSpeed.speed > 0 ? targetAlpha : startAlpha;
            var handle = LMotion.Create(fromAlpha, toAlpha, calculatedDuration)
                .WithEase(LitMotion.Ease.OutQuad).WithScheduler(Timing.GetScheduler())
                .BindToAlpha(targetCanvasGroup);
            await handle.ToUniTask(cancellationToken);
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            float currentAlpha = targetCanvasGroup.alpha;
            float calculatedDuration = Timing.Duration * Mathf.InverseLerp(startAlpha,targetAlpha, currentAlpha) / Mathf.Abs(playbackSpeed.rewindSpeed);
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

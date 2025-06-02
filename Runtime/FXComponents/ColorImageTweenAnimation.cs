using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorImageTweenAnimation : BaseFXComponent
    {
        [SerializeField] Image imageToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Color fromColor = inheritedSpeed > 0 ? startColor : targetColor;
            Color toColor = inheritedSpeed > 0 ? targetColor : startColor;
            var handle = LMotion.Create(fromColor, toColor, calculatedDuration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(imageToColor);

            await handle.ToUniTask(cancellationToken);
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Color currentColor = imageToColor.color;
            var handle = LMotion.Create(currentColor, startColor, calculatedDuration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(imageToColor);

            await handle.ToUniTask();
        }

        protected override void StopInternal()
        {
            imageToColor.color = startColor;
        }

        protected override void ResetInternal()
        {
            imageToColor.color = startColor;
        }
    }
}

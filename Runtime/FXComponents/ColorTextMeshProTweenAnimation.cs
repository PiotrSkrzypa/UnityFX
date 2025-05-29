using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorTextMeshProTweenAnimation : BaseFXComponent
    {
        [SerializeField] TMP_Text textToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            var handle = LMotion.Create(startColor, targetColor, calculatedDuration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(textToColor);

            await handle.ToUniTask(cancellationToken);
        }
        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Color currentColor = textToColor.color;
            var handle = LMotion.Create(currentColor, startColor, calculatedDuration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(textToColor);

            await handle.ToUniTask();
        }

        protected override void StopInternal()
        {
            textToColor.color = startColor;
        }

        protected override void ResetInternal()
        {
            textToColor.color = startColor;
        }
    }
}

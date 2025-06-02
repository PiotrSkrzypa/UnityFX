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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.speed);
            Color fromColor = playbackSpeed.speed > 0 ? startColor : targetColor;
            Color toColor = playbackSpeed.speed > 0 ? targetColor : startColor;
            var handle = LMotion.Create(fromColor, toColor, calculatedDuration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(textToColor);

            await handle.ToUniTask(cancellationToken);
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.rewindSpeed);
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

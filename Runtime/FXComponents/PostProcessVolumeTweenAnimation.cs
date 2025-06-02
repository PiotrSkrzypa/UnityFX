using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PostProcessVolumeTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Volume volume;
        [SerializeField] private float startWeight = 0f;
        [SerializeField] private float targetWeight = 1f;
        [SerializeField] private Ease ease = Ease.OutQuad;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            if (volume == null)
            {
                Debug.LogWarning("[PostProcessVolumeTweenAnimation] Volume is null.");
                return;
            }
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.speed);
            float fromWeight = playbackSpeed.speed > 0 ? startWeight : targetWeight;
            float toWeight = playbackSpeed.speed > 0 ? targetWeight : startWeight;
            volume.weight = fromWeight;

            var scheduler = Timing.GetScheduler();

            var tween = LMotion.Create(fromWeight, toWeight, calculatedDuration)
                .WithEase(ease)
                .WithScheduler(scheduler)
                .Bind(volume, (x, v) => v.weight = x);

            await tween.ToUniTask(cancellationToken);
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.rewindSpeed);
            float currentWeight = volume.weight;
            var scheduler = Timing.GetScheduler();

            var tween = LMotion.Create(currentWeight, startWeight, calculatedDuration)
                .WithEase(ease)
                .WithScheduler(scheduler)
                .Bind(volume, (x, v) => v.weight = x);

            await tween.ToUniTask();
        }
        protected override void StopInternal()
        {
            if (volume != null)
                volume.weight = startWeight;
        }

        protected override void ResetInternal()
        {
            if (volume != null)
                volume.weight = startWeight;
        }
    }
}

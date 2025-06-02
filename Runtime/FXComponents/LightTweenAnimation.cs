using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class LightTweenAnimation : BaseFXComponent
    {
        [SerializeField] Light light;
        [SerializeField] float startIntensity = 0f;
        [SerializeField] float targetIntensity = 1f;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] Light reference is null.");
                return;
            }
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.speed);
            light.intensity = startIntensity;

            float elapsed = 0f;

            while (elapsed < calculatedDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                float t = elapsed / calculatedDuration;
                light.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                await UniTask.Yield(PlayerLoopTiming.Update);

                elapsed += Timing.TimeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            light.intensity = targetIntensity;
        }
        protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] Light reference is null.");
                return;
            }
            float calculatedDuration = Timing.Duration / Mathf.Abs(playbackSpeed.rewindSpeed);
            float currentIntensity = light.intensity;

            float elapsed = 0f;

            while (elapsed < calculatedDuration)
            {
                float t = elapsed / calculatedDuration;
                light.intensity = Mathf.Lerp(currentIntensity, startIntensity, t);
                await UniTask.Yield(PlayerLoopTiming.Update);

                elapsed += Timing.TimeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            light.intensity = startIntensity;
        }

        protected override void StopInternal()
        {
            if (light != null)
                light.intensity = startIntensity;
        }

        protected override void ResetInternal()
        {
            if (light != null)
                light.intensity = startIntensity;
        }
    }
}

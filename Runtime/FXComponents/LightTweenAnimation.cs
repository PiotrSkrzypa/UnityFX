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

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] Light reference is null.");
                return;
            }
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
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
        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] Light reference is null.");
                return;
            }
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
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

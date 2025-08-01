using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Light/Intensity")]
    public class FXLightIntensityAnimation : FXAnimationComponent
    {
        [SerializeField] Light light;
        [SerializeField] float startIntensity = 0f;
        [SerializeField] float targetIntensity = 1f;

        protected override void Initialize()
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] light is null.");
                return;
            }
        }
        protected override void OnUpdate(float progress)
        {
            light.intensity = Mathf.LerpUnclamped(startIntensity, targetIntensity, progress);
        }

        protected override void Reset()
        {
            if (light != null)
                light.intensity = startIntensity;
        }

    }
}

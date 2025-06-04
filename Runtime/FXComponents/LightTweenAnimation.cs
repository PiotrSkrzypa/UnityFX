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

        public override void Initialize()
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] light is null.");
                return;
            }
        }
        protected override void Update(float progress)
        {
            light.intensity = Mathf.LerpUnclamped(startIntensity, targetIntensity, progress);
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

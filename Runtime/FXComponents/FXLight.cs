using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXLight : BaseFXComponent
    {
        [SerializeField] Light light;

        public override void Initialize()
        {
            if (light == null)
            {
                Debug.LogWarning("[FXLight] light is null.");
                return;
            }
        }
        protected override void Update(float progress)
        {
            if (progress == 0f)
            {
                light.enabled = false;
            }
            if (progress == 1f)
            {
                light.enabled = true;
            }
        }

        protected override void StopInternal()
        {
            light.enabled = false;
        }
        protected override void ResetInternal()
        {
            light.enabled = false;
        }
    }
}

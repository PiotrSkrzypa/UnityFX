using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXTrailRenderer : BaseFXComponent
    {
        [SerializeField] TrailRenderer trailRenderer;

        protected override void Update(float progress)
        {
            if(progress== 0f)
            {
                trailRenderer.emitting = false;
            }
            else if (progress == 1f)
            {
                trailRenderer.emitting = true;
            }
        }

        protected override void StopInternal()
        {
            trailRenderer.emitting = false;
        }
        protected override void ResetInternal()
        {
            trailRenderer.emitting = false;
        }
    }
}

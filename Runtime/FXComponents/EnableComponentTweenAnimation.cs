using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class EnableComponentTweenAnimation : BaseFXComponent
    {
        [SerializeField] Behaviour componentToEnable;
        [SerializeField] bool targetState = true;

        bool originalState;

        public override void Initialize()
        {
            if (componentToEnable == null)
            {
                Debug.LogWarning("[EnableComponentTweenAnimation] componentToEnable is null.");
                return;
            }
            originalState = componentToEnable.enabled;
        }
        protected override void Update(float progress)
        {
            if (progress == 0f)
            {
                componentToEnable.enabled = originalState;
            }
            if (progress == 1f)
            {
                componentToEnable.enabled = targetState;
            }
        }

        protected override void StopInternal()
        {
            if (componentToEnable != null)
            {
                componentToEnable.enabled = originalState;
            }
        }
        protected override void ResetInternal()
        {
            if (componentToEnable != null)
            {
                componentToEnable.enabled = originalState;
            }
        }
    }
}

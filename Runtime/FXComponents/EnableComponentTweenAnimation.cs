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


        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            if (componentToEnable == null)
            {
                Debug.LogWarning("[EnableComponentTweenAnimation] componentToEnable is null.");
                return;
            }
            originalState = componentToEnable.enabled;
            componentToEnable.enabled = targetState;
            await UniTask.CompletedTask;
        }
        protected override async UniTask Rewind(float inheritedSpeed = 1)
        {
            if (componentToEnable == null)
            {
                Debug.LogWarning("[EnableComponentTweenAnimation] componentToEnable is null.");
                return;
            }
            componentToEnable.enabled = originalState;
            await UniTask.CompletedTask;
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

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CanvasGroupAlphaTweenAnimation : BaseFXComponent
    {
        [SerializeField] CanvasGroup targetCanvasGroup;
        [SerializeField] float startAlpha;
        [SerializeField] float targetAlpha;

        protected override void Update(float progress)
        {
            targetCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
        }
        protected override void StopInternal()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
        protected override void ResetInternal()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
    }
}

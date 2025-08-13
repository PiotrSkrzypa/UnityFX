using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("UI/Canvas Group/Alpha")]
    public class FXCanvasGroupAlphaAnimation : FXAnimationComponent
    {
        [SerializeField] CanvasGroup targetCanvasGroup;
        [SerializeField] float startAlpha;
        [SerializeField] float targetAlpha;

        protected override void OnUpdate(float progress)
        {
            targetCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
        }
        protected override void Reset()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
    }
}

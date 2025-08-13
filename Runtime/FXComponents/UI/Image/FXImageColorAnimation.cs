using System;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("UI/Image/Color")]
    public sealed class ColorImageTweenAnimation : FXAnimationComponent
    {
        [SerializeField] Image imageToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override void OnUpdate(float progress)
        {
            imageToColor.color = Color.LerpUnclamped(startColor, targetColor, progress);
        }

        protected override void Reset()
        {
            imageToColor.color = startColor;
            isInitialized = false;
        }
    }
}

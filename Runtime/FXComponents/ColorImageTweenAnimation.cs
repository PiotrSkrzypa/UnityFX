using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorImageTweenAnimation : BaseFXComponent
    {
        [SerializeField] Image imageToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override void Update(float progress)
        {
            imageToColor.color = Color.LerpUnclamped(startColor, targetColor, progress);
        }

        protected override void StopInternal()
        {
            imageToColor.color = startColor;
        }

        protected override void ResetInternal()
        {
            imageToColor.color = startColor;
        }
    }
}

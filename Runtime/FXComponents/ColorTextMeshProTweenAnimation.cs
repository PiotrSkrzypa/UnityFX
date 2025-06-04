using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorTextMeshProTweenAnimation : BaseFXComponent
    {
        [SerializeField] TMP_Text textToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override void Update(float progress)
        {
            textToColor.color = Color.Lerp(startColor, targetColor, progress);
        }

        protected override void StopInternal()
        {
            textToColor.color = startColor;
        }

        protected override void ResetInternal()
        {
            textToColor.color = startColor;
        }
    }
}

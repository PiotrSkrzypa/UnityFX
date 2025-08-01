using System;
using TMPro;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("UI/Text Mesh Pro/Color")]
    public class FXColorTMPAnimation : FXAnimationComponent
    {
        [SerializeField] TMP_Text textToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override void OnUpdate(float progress)
        {
            textToColor.color = Color.LerpUnclamped(startColor, targetColor, progress);
        }

        protected override void Reset()
        {
            textToColor.color = startColor;
        }
    }
}

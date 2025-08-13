using System;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXGradientColorGraphicAnimation : FXAnimationComponent
    {
        [SerializeField] Graphic graphicToColor;
        [SerializeField] float gradientSamplingResolution = 30f;
        [SerializeField] Gradient gradient;

        Color[] colorSamples;
        Color originalColor;

        protected override void Initialize()
        {
            originalColor = graphicToColor.color;
            colorSamples = SampleGradient();
        }

        protected override void OnUpdate(float progress)
        {
            var color = GetInterpolatedSampledColor(progress);
            graphicToColor.color = color;
        }

        private Color[] SampleGradient()
        {
            int steps = Mathf.Max(1, Mathf.RoundToInt(gradientSamplingResolution));
            Color[] result = new Color[steps];

            for (int i = 0; i < steps; i++)
            {
                float t = i / (steps - 1f);
                result[i] = gradient.Evaluate(t);
            }

            return result;
        }
        private Color GetInterpolatedSampledColor(float t)
        {
            float rawIndex = t * (colorSamples.Length - 1);
            int indexA = Mathf.FloorToInt(rawIndex);
            int indexB = Mathf.Min(indexA + 1, colorSamples.Length - 1);
            float lerpT = rawIndex - indexA;
            Color interpolated = Color.LerpUnclamped(colorSamples[indexA], colorSamples[indexB], lerpT);
            return interpolated;
        }
        protected override void Reset()
        {
            if (graphicToColor != null)
            {
                graphicToColor.color = originalColor;
            }
        }

    }
}

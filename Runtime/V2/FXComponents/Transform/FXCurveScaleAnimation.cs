using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Transform/Curve Scale")]
    [FXComponentIcon("d_Transform Icon")]
    public class FXCurveScaleAnimation : FXAnimationComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] Vector3 startingScale = Vector3.one;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xCurve;
        [SerializeField] AnimationCurve yCurve;
        [SerializeField] AnimationCurve zCurve;
        Vector3[] sampledCurvesValues;


        protected override void Initialize()
        {
            sampledCurvesValues = SampleCurves();
        }
        protected override void OnUpdate(float progress)
        {
            var scale = GetInterpolatedSampledRotation(progress);
            targetTransform.localScale = scale;
        }
        
        private Vector3 GetInterpolatedSampledRotation(float t)
        {
            float rawIndex = t * (sampledCurvesValues.Length - 1);
            int indexA = Mathf.FloorToInt(rawIndex);
            int indexB = Mathf.Min(indexA + 1, sampledCurvesValues.Length - 1);
            float lerpT = rawIndex - indexA;
            Vector3 interpolated = Vector3.LerpUnclamped(sampledCurvesValues[indexA], sampledCurvesValues[indexB], lerpT);
            return interpolated;
        }

        private Vector3[] SampleCurves()
        {
            int steps = Mathf.Max(2, Mathf.RoundToInt(samplingResolution));
            Vector3[] result = new Vector3[steps];
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)(steps - 1);
                result[i] = new Vector3(
                    xCurve?.Evaluate(t) ?? 0f,
                    yCurve?.Evaluate(t) ?? 0f,
                    zCurve?.Evaluate(t) ?? 0f
                );
            }
            return result;
        }
        protected override void Reset()
        {
            targetTransform.localScale = startingScale;
        }
    }
}

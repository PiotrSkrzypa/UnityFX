using System;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Transform/Curve Position")]
    [FXComponentIcon("d_Transform Icon")]
    public sealed class FXCurvePositionTweenAnimation : FXAnimationComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] Vector3 startingPosition;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xPositionCurve;
        [SerializeField] AnimationCurve yPositionCurve;
        [SerializeField] AnimationCurve zPositionCurve;
        Vector3[] sampledPath;

        protected override void Initialize()
        {
            sampledPath = SampleCurves();
        }
        protected override void OnUpdate(float progress)
        {
            Vector3 interpolated = GetInterpolatedSampledPosition(progress);
            if (useLocalSpace)
                targetTransform.localPosition = interpolated;
            else
                targetTransform.position = interpolated;
        }

        private Vector3 GetInterpolatedSampledPosition(float t)
        {
            float rawIndex = t * (sampledPath.Length - 1);
            int indexA = Mathf.FloorToInt(rawIndex);
            indexA = Mathf.Clamp(indexA, 0, sampledPath.Length - 1);
            int indexB = Mathf.Min(indexA + 1, sampledPath.Length - 1);
            float lerpT = rawIndex - indexA;
            Vector3 interpolated = Vector3.LerpUnclamped(sampledPath[indexA], sampledPath[indexB], lerpT);
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
                    xPositionCurve?.Evaluate(t) ?? 0f,
                    yPositionCurve?.Evaluate(t) ?? 0f,
                    zPositionCurve?.Evaluate(t) ?? 0f
                );
            }
            return result;
        }

        protected override void Reset()
        {
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;
            isInitialized = false;
        }
    }
}

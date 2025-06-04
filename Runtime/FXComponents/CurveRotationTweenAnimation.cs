using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurveRotationTweenAnimation : BaseFXComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] Vector3 startingRotation;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xCurve;
        [SerializeField] AnimationCurve yCurve;
        [SerializeField] AnimationCurve zCurve;
        Vector3[] sampledCurvesValues;

        public override void Initialize()
        {
            sampledCurvesValues = SampleCurves();
        }

        protected override void Update(float progress)
        {
            var rot = GetInterpolatedSampledRotation(progress);

            if (useLocalSpace)
                targetTransform.localEulerAngles = rot;
            else
                targetTransform.eulerAngles = rot;
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
        protected override void StopInternal()
        {
            if (useLocalSpace)
                targetTransform.localEulerAngles = startingRotation;
            else
                targetTransform.eulerAngles = startingRotation;
        }

        protected override void ResetInternal()
        {
            if (useLocalSpace)
                targetTransform.localEulerAngles = startingRotation;
            else
                targetTransform.eulerAngles = startingRotation;
        }
    }
}

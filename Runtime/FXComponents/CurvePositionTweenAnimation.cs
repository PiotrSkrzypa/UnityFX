using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurvePositionTweenAnimation : BaseFXComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] Vector3 startingPosition;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xPositionCurve;
        [SerializeField] AnimationCurve yPositionCurve;
        [SerializeField] AnimationCurve zPositionCurve;
        [SerializeField] Ease easeType = Ease.Linear;
        Vector3[] sampledPath;
        float progress;

        public override void Initialize()
        {
            sampledPath = SampleCurves();
        }

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;

            await LMotion.Create(0f, 1f, calculatedDuration)
                .WithScheduler(Timing.GetScheduler())
                .WithEase(easeType)
                .Bind(t =>
                {
                    progress = t;
                    Vector3 interpolated = GetInterpolatedSampledPosition(t);
                    if (useLocalSpace)
                        targetTransform.localPosition = interpolated;
                    else
                        targetTransform.position = interpolated;
                })
                .ToUniTask(cancellationToken);
        }

        private Vector3 GetInterpolatedSampledPosition(float t)
        {
            float rawIndex = t * (sampledPath.Length - 1);
            int indexA = Mathf.FloorToInt(rawIndex);
            int indexB = Mathf.Min(indexA + 1, sampledPath.Length - 1);
            float lerpT = rawIndex - indexA;
            Vector3 interpolated = Vector3.LerpUnclamped(sampledPath[indexA], sampledPath[indexB], lerpT);
            return interpolated;
        }

        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);

            await LMotion.Create(progress, 0f, calculatedDuration)
                .WithScheduler(Timing.GetScheduler())
                .WithEase(easeType)
                .Bind(t =>
                {
                    Vector3 interpolated = GetInterpolatedSampledPosition(t);
                    if (useLocalSpace)
                        targetTransform.localPosition = interpolated;
                    else
                        targetTransform.position = interpolated;
                })
                .ToUniTask();
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

        protected override void StopInternal()
        {
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;
        }

        protected override void ResetInternal()
        {
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;
        }
    }
}

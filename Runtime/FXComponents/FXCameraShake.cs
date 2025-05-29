using UnityEngine;
using System;
using LitMotion;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXCameraShake : BaseFXComponent
    {
        [SerializeField] int frequency = 10;
        [SerializeField] float damping = 0.5f;
        [SerializeField] bool fadeOut;
        [SerializeField] Ease easeType = Ease.OutQuad;
        [SerializeField] Vector3 movementShakeMagnitude;
        [SerializeField] float zRotationShakeMagnitude;

        Vector3 originalPosiiton;
        Vector3 originalRotation;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Camera camera = Camera.main;
            originalPosiiton = camera.transform.localPosition;
            originalRotation = camera.transform.localEulerAngles;
            List<UniTask> tasks = new List<UniTask>();
            var scheduler = Timing.GetScheduler();
            if (movementShakeMagnitude != Vector3.zero)
            {
                UniTask uniTask = LMotion.Shake.Create(originalPosiiton, movementShakeMagnitude, calculatedDuration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(camera.transform, (v, tr) => tr.localPosition = v)
                .ToUniTask(cancellationToken);
                tasks.Add(uniTask);
            }
            if (zRotationShakeMagnitude != 0)
            {
                Vector3 rotationMagnitude = new Vector3(0, 0, zRotationShakeMagnitude);
                UniTask uniTask = LMotion.Shake.Create(originalRotation, rotationMagnitude, calculatedDuration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(camera.transform, (v, tr) => tr.localEulerAngles = v)
                .ToUniTask(cancellationToken);
                tasks.Add(uniTask);
            }
            await UniTask.WhenAll(tasks);
        }
        protected override async UniTask Reverse(float inheritedSpeed = 1)
        {
            float calculatedDuration = Timing.Duration / Mathf.Abs(inheritedSpeed);
            Camera camera = Camera.main;
            List<UniTask> tasks = new List<UniTask>();
            var scheduler = Timing.GetScheduler();
            if (movementShakeMagnitude != Vector3.zero)
            {
                UniTask uniTask = LMotion.Shake.Create(originalPosiiton, movementShakeMagnitude, calculatedDuration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(camera.transform, (v, tr) => tr.localPosition = v)
                .ToUniTask();
                tasks.Add(uniTask);
            }
            if (zRotationShakeMagnitude != 0)
            {
                Vector3 rotationMagnitude = new Vector3(0, 0, zRotationShakeMagnitude);
                UniTask uniTask = LMotion.Shake.Create(originalRotation, rotationMagnitude, calculatedDuration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(camera.transform, (v, tr) => tr.localEulerAngles = v)
                .ToUniTask();
                tasks.Add(uniTask);
            }
            await UniTask.WhenAll(tasks);
        }

    }
}
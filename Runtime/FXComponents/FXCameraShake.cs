using UnityEngine;
using System;
using LitMotion;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using static LitMotion.LMotion;

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

        Camera camera;
        Transform cameraTransform;
        Vector3 originalPosiiton;
        Vector3 originalRotation;
        Vector3 rotationMagnitude;

        public override void Initialize()
        {
            camera = Camera.main;
            cameraTransform = camera.transform;
            rotationMagnitude = new Vector3(0, 0, zRotationShakeMagnitude);
            originalPosiiton = camera.transform.localPosition;
            originalRotation = camera.transform.localEulerAngles;
        }
        protected override void Update(float progress)
        {
            float angularFrequency = (frequency - 0.5f) * Mathf.PI;
            float dampingFactor = damping * frequency / (2f * Mathf.PI);
            Vector3 positionOffset = Mathf.Cos(angularFrequency * progress) * Mathf.Pow(Mathf.Epsilon, -dampingFactor * progress) * movementShakeMagnitude;
            Vector3 rotationOffset = Mathf.Cos(angularFrequency * progress) * Mathf.Pow(Mathf.Epsilon, -dampingFactor * progress) * rotationMagnitude;
            cameraTransform.localPosition = originalPosiiton + positionOffset;
            cameraTransform.localEulerAngles = originalRotation + rotationOffset;

        }

    }
}
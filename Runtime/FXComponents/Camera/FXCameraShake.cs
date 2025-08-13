using UnityEngine;
using System;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    [FXComponent("Camera/Shake")]
    public class FXCameraShake : FXAnimationComponent
    {
        private const float PI = Mathf.PI;
        [SerializeField] int frequency = 10;
        [SerializeField] float damping = 0.5f;
        [SerializeField] bool fadeOut;
        [SerializeField] Vector3 movementShakeMagnitude;
        [SerializeField] float zRotationShakeMagnitude;

        Camera camera;
        Transform cameraTransform;
        Vector3 originalPosiiton;
        Vector3 originalRotation;
        Vector3 rotationMagnitude;

        protected override void Initialize()
        {
            camera = Camera.main;
            cameraTransform = camera.transform;
            rotationMagnitude = new Vector3(0, 0, zRotationShakeMagnitude);
            originalPosiiton = camera.transform.localPosition;
            originalRotation = camera.transform.localEulerAngles;
        }
        protected override void OnUpdate(float progress)
        {
            float t = (progress - 0.5f) * 2f;
            float angularFrequency = PI * frequency;
            float dampingFactor = damping * frequency / (2f * PI);

            float oscillation = Mathf.Sin(progress * angularFrequency);
            float decay = Mathf.Exp(-dampingFactor * Mathf.Abs(t));

            Vector3 positionOffset =  oscillation * decay * movementShakeMagnitude;
            Vector3 rotationOffset =  oscillation * decay * rotationMagnitude;
            cameraTransform.localPosition = originalPosiiton + positionOffset;
            cameraTransform.localEulerAngles = originalRotation + rotationOffset;

        }

    }
}
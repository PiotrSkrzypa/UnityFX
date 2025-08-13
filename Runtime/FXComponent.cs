using System;
using System.Linq;
using System.Reflection;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public abstract partial class FXComponent
    {
        // TODO Add cooldown
        [SerializeField] string displayName;
        [SerializeField] bool enabled = true;
        protected bool isPlaying = false;
        protected bool isInitialized = false;
        private float progress = 0f;

        public bool Enabled => enabled;
        public string DisplayName => displayName;
        public bool IsPlaying => isPlaying;

        protected FXCallbackActions callbackActions = new FXCallbackActions();
        

        protected FXCallbackActions AccessCallbacks(FXComponent other)
        {
            return other.callbackActions;
        }

        public abstract void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1f);

        protected virtual void Initialize() { }
        protected virtual void OnUpdate(float progress) { }
        public virtual void Resume() { }
        public virtual void Pause() { }
        public virtual void Rewind(float playbackSpeedModifier = 1f) { }
        public virtual void Stop() { }

        public MotionHandle TrackedHandle { get => trackedHandle; set => trackedHandle = value; }
        public float Progress { get => GetProgress(); protected set => progress = value; }


        protected MotionHandle trackedHandle;

        public FXComponent()
        {
#if UNITY_EDITOR
            var type = GetType();
            var attribute = type.GetCustomAttribute<FXComponentAttribute>();
            displayName = attribute != null
                ? attribute.MenuName.Split('/').Last()
                : type.Name;
#endif
        }

        protected virtual float GetProgress()
        {
            return progress;
        }
    }
}
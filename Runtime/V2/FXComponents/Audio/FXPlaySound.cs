using System;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Audio/Play Sound")]
    public sealed class FXPlaySound : FXComponent
    {
        [SerializeField] AudioClip audioClip;
        [SerializeField] string uiAudioClipKey;
        [SerializeField] bool isUISound;
#if UNITY_EDITOR
        private GameObject audioPlayerGO;
        private AudioSource audioSource;
#endif

        protected override void Initialize()
        {
            if (audioClip == null)
            {
                Debug.LogWarning("[FXAudioEffect] audio clip is null.");
                return;
            }
        }
        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
        {
            if (playbackDirection == PlaybackDirection.Forward)
            {
                if (Application.isPlaying)
                {
                    AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
                }
#if UNITY_EDITOR
                else
                {
                    HandleEditorPlayback();
                }
#endif
            }
            callbackActions.OnCompleteAction?.Invoke();
        }

#if UNITY_EDITOR
        private void HandleEditorPlayback()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            if (audioPlayerGO == null)
            {
                audioPlayerGO = new GameObject("EditModeAudioPlayer");
                audioPlayerGO.hideFlags = HideFlags.HideAndDontSave;
                audioSource = audioPlayerGO.AddComponent<AudioSource>();
            }
            audioSource.clip = audioClip;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.volume = 1.0f;
            audioSource.Play();
        }
#endif
#if UNITY_EDITOR
        private void OnEditorUpdate()
        {
            if (audioSource != null && !audioSource.isPlaying && audioPlayerGO != null)
            {
                GameObject.DestroyImmediate(audioPlayerGO);
                audioPlayerGO = null;
                audioSource = null;
                EditorApplication.update -= OnEditorUpdate;
            }
        }
#endif
        public override void Rewind(float playbackSpeedModifier = 1)
        {
            callbackActions.OnRewindCompleteAction?.Invoke();
        }
    }
}
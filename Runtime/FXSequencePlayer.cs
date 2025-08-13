using UnityEngine;

namespace PSkrzypa.UnityFX
{
    public class FXSequencePlayer : MonoBehaviour
    {
        [SerializeField] bool playOnAwake;
        [SerializeField] bool playOnEnable;
        [SerializeField] bool stopOnDisable;
        [SerializeField] FXSequence fXSequence;

        public bool IsPlaying => fXSequence != null && fXSequence.IsPlaying;

        public FXSequence FXSequence { get => fXSequence; }

        private void Awake()
        {
            Stop();
        }
        public void Play()
        {
            if (fXSequence == null)
            {
                Debug.LogError("FXSequenceComponent is not assigned.", this);
                return;
            }
            fXSequence.Play(PlaybackDirection.Forward);
        }
        public void Resume()
        {
            fXSequence.Resume();
        }
        public void Pause()
        {
            fXSequence.Pause();
        }
        public void Rewind()
        {
            fXSequence.Rewind();
        }
        public void Stop()
        {
            fXSequence.Stop();
        }
    }
}
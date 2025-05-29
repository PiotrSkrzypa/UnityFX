using UnityEngine;
using Alchemy.Inspector;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

namespace PSkrzypa.UnityFX
{
    public class FXPlayer : MonoBehaviour
    {
        [SerializeField] bool playOnAwake;
        [SerializeField] bool playOnEnable;
        [SerializeField] bool stopOnDisable;
        [SerializeField][SerializeReference] FXSequence sequence = new FXSequence();
        public FXSequence Sequence { get => sequence; }

        public UnityEvent OnPlay;
        public UnityEvent OnCompleted;
        public UnityEvent OnStopped;
        public UnityEvent OnCancelled;

        protected bool initialized;

        public bool IsPlaying { get; protected set; }
        public bool IsCancelled { get; protected set; }

        protected virtual void Awake()
        {
            if (!initialized)
            {
                Initialize();
            }
            if (playOnAwake)
            {
                Play();
            }
        }
        public void Initialize()
        {
            initialized = true;
        }
        private void OnEnable()
        {
            if (!initialized)
            {
                Initialize();
            }
            if (playOnEnable)
            {
                Play();
            }
        }
        private void OnDisable()
        {
            if (initialized && stopOnDisable)
            {
                Stop();
            }
        }
        [Button]
        public void Play() 
        {
             Play(1f).Forget();
        }
        public async UniTaskVoid Play(float inheritedSpeed = 1f)
        {
            if (sequence != null)
            {
                IsPlaying = true;
                OnPlay?.Invoke();
                sequence.Initialize();
                UniTask task = sequence.Play(inheritedSpeed);
                await task;
                if (IsCancelled)
                {
                    IsCancelled = false;
                }
                else
                {
                    OnCompleted?.Invoke();
                }
                IsPlaying = false;
            }
        }
        public async UniTask Play(CancellationToken cancellationToken, float inheritedSpeed = 1f)
        {
            if (sequence != null)
            {
                IsPlaying = true;
                OnPlay?.Invoke();
                try
                {
                    sequence.Initialize();
                    UniTask task = sequence.Play(inheritedSpeed);
                    await task.AttachExternalCancellation(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    sequence.Cancel();
                }
                if (IsCancelled)
                {
                    IsCancelled = false;
                }
                else
                {
                    OnCompleted?.Invoke();
                }
                IsPlaying = false;
            }
        }
        [Button]
        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }
            if (sequence == null)
            {
                return;
            }
            sequence.Stop();
            IsPlaying = false;
            OnStopped?.Invoke();
        }
        [Button]
        public void Cancel()
        {
            if (!IsPlaying)
            {
                return;
            }
            if (sequence == null)
            {
                return;
            }
            sequence.Cancel();
            IsCancelled = true;
            OnCancelled?.Invoke();
        }
        [Button]
        public void ResetComponents()
        {
            if (sequence == null)
            {
                return;
            }
            sequence.Reset();
        }
    } 
}

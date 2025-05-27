using UnityEngine;
using Alchemy.Inspector;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace PSkrzypa.UnityFX
{
    public class FXPlayer : MonoBehaviour
    {
        [SerializeField] bool playOnAwake;
        [SerializeField] bool playOnEnable;
        [SerializeField] bool stopOnDisable;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] bool forceTimeScaleSettingOnComponents = true;
        [SerializeField][SerializeReference] IFXComponent[] components;
        public IFXComponent[] Components { get => components; }

        public UnityEvent OnPlay;
        public UnityEvent OnCompleted;
        public UnityEvent OnCancelled;

        protected bool initialized;

        public bool IsPlaying { get; protected set; }

        protected virtual void Awake()
        {
            if (!initialized)
            {
                Initialize();
            }
            if (playOnAwake)
            {
                Play().Forget();
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
                Play().Forget();
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
        public async UniTaskVoid Play()
        {
            if (components != null)
            {
                IsPlaying = true;
                OnPlay?.Invoke();
                List<UniTask> tasks = new List<UniTask>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    UniTask task = components[i].Play();
                    if(components[i].Timing.ContributeToTotalDuration)
                    {
                        tasks.Add(task);
                    }
                }
                await UniTask.WhenAll(tasks);
                OnCompleted?.Invoke();
            }
        }
        public async UniTask Play(CancellationToken cancellationToken)
        {
            if (components != null)
            {
                IsPlaying = true;
                OnPlay?.Invoke();
                List<UniTask> tasks = new List<UniTask>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    UniTask task = components[i].Play();
                    if (components[i].Timing.ContributeToTotalDuration)
                    {
                        tasks.Add(task);
                    }
                }
                await UniTask.WhenAll(tasks).AttachExternalCancellation(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                OnCompleted?.Invoke();
            }
        }
        [Button]
        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }
            if (components == null)
            {
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Stop();
            }
            IsPlaying = false;
            OnCancelled?.Invoke();
        }
        [Button]
        public void ResetComponents()
        {
            if (components == null)
            {
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Reset();
            }
        }
    } 
}

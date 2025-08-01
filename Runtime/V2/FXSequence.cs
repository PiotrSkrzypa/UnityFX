using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitMotion;
using UnityEngine;
using UnityEngine.Events;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    public sealed class FXSequence : FXComponent
    {
        [SerializeField] FXSequenceSettings settings;
        [SerializeField][SerializeReference] FXComponent[] components;

        Queue<FXComponent> waitingComponents = new Queue<FXComponent>();
        Stack<FXComponent> completedComponents = new Stack<FXComponent>();
        List<FXComponent> playingComponents = new List<FXComponent>();

        int startedLoopsCount = 0;
        PlaybackDirection lastPlaybackDirection = PlaybackDirection.Forward;
        float lastPlaybackSpeedModifier = 1f;
        CancellationTokenSource cts;
        FXComponent longestComponent;

        public FXComponent[] Components { get => components; }

        public UnityEvent OnCompleteEvent { get => onComplete; set => onComplete =  value ; }
        [SerializeField] UnityEvent onComplete;

        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1f)
        {
            lastPlaybackDirection = playbackDirection;
            ModifyLoopsCounter(playbackDirection);
            switch(lastPlaybackDirection)
            {
                case PlaybackDirection.Forward:
                    lastPlaybackSpeedModifier = playbackSpeedModifier * settings.ForwardPlaybackSpeed;
                    break;
                case PlaybackDirection.Backward:
                    lastPlaybackSpeedModifier = playbackSpeedModifier * settings.BackwardPlaybackSpeed;
                    break;
            }
            switch (settings.SequencePlayMode)
            {
                case SequencePlayMode.Parallel:
                    PlayInParallel(playbackDirection, playbackSpeedModifier);
                    break;
                case SequencePlayMode.Sequential:
                    PlaySequentially();
                    break;
            }
            isPlaying = true;
        }
        protected override float GetProgress()
        {
            float totalProgress = 0f;
            switch (settings.SequencePlayMode)
            {
                case SequencePlayMode.Parallel:
                    {
                        totalProgress = GetLongestComponentProgress();
                        break;
                    }

                default:
                    {
                        for (int index = 0; index < components.Length; index++)
                        {
                            var component = components[index];
                            if (component == null) continue;
                            if (!component.Enabled) continue;
                            totalProgress += component.Progress;
                        }
                        totalProgress /= components.Length;
                        break;
                    }
            }
            return totalProgress;
        }

        private float GetLongestComponentProgress()
        {
            float totalProgress;
            if (longestComponent == null)
            {
                longestComponent = FindLongestComponent();
            }
            totalProgress = longestComponent.Progress;
            return totalProgress;
        }

        private FXComponent FindLongestComponent()
        {
            int longestComponentIndex = 0;
            double longestComponentDuration = 0f;
            for (int index = 0; index < components.Length; index++)
            {
                var component = components[index];
                if (component == null) continue;
                if (!component.Enabled) continue;
                if (component.TrackedHandle.TotalDuration > longestComponentDuration)
                {
                    longestComponentDuration = component.TrackedHandle.TotalDuration;
                    longestComponentIndex = index;
                }
            }
            return components[longestComponentIndex];
        }

        private void ModifyLoopsCounter(PlaybackDirection playbackDirection)
        {
            switch (playbackDirection)
            {
                case PlaybackDirection.Forward:
                    startedLoopsCount++;
                    break;
                case PlaybackDirection.Backward:
                    startedLoopsCount--;
                    break;
            }
        }

        private void PlayInParallel(PlaybackDirection playbackDirection, float playbackSpeedModifier)
        {
            for (int index = 0; index < components.Length; index++)
            {
                var component = components[index];
                if (component == null) continue;
                if (!component.Enabled) continue;

                component.Play(playbackDirection, playbackSpeedModifier);

                if (!isPlaying)
                {
                    playingComponents.Add(component);
                }
            }

            WaitForComponentsToFinish().Forget();
            isPlaying = true;
        }

        async UniTaskVoid WaitForComponentsToFinish()
        {
            CancellationTokenCleanUp();
            cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;

            PlayerLoopTiming playerLoopTiming = settings.TimescaleIndependent ? PlayerLoopTiming.TimeUpdate : PlayerLoopTiming.Update;
            await UniTask.WaitUntil(() => playingComponents.TrueForAll(x => !x.IsPlaying), playerLoopTiming, cancellationToken);
            OnSequenceLoopCompleted();
        }
        private void CancellationTokenCleanUp()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
        private void PlaySequentially()
        {
            if (!isPlaying)
            {
                waitingComponents.Clear();
                completedComponents.Clear();
                for (int index = 0; index < components.Length; index++)
                {
                    var component = components[index];
                    if (component == null) continue;
                    if (lastPlaybackDirection == PlaybackDirection.Forward)
                    {
                        waitingComponents.Enqueue(component);
                    }
                    else
                    {
                        completedComponents.Push(component);
                    }
                }
            }
            switch (lastPlaybackDirection)
            {
                case PlaybackDirection.Forward:
                    PlayWaitingComponents();
                    break;
                case PlaybackDirection.Backward:
                    RewindCompletedComponents();
                    break;
            }
        }

        void PlayWaitingComponents()
        {
            FXComponent currentlyPlayedComponent = playingComponents.Count > 0 ? playingComponents[0] : null;
            if (currentlyPlayedComponent != null)
            {
                if (currentlyPlayedComponent.IsPlaying)
                {
                    currentlyPlayedComponent.Play(lastPlaybackDirection, lastPlaybackSpeedModifier);
                    return;
                }
                else
                {
                    completedComponents.Push(currentlyPlayedComponent);
                    AccessCallbacks(currentlyPlayedComponent).OnCompleteAction.Unsubscribe(PlayWaitingComponents);
                    playingComponents.Remove(currentlyPlayedComponent);

                }
            }
            if (waitingComponents.TryDequeue(out var queuedComponent))
            {
                try
                {
                    if (queuedComponent.Enabled)
                    {
                        AccessCallbacks(queuedComponent).OnCompleteAction.Subscribe(PlayWaitingComponents);
                        queuedComponent.Play(lastPlaybackDirection, lastPlaybackSpeedModifier);
                        playingComponents.Add(queuedComponent);

                    }
                    else
                    {
                        PlayWaitingComponents();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                OnSequenceLoopCompleted();
            }
        }

        private void RewindCompletedComponents()
        {
            FXComponent currentlyPlayingComponent = playingComponents.Count > 0 ? playingComponents[0] : null;
            if (currentlyPlayingComponent != null)
            {
                if (currentlyPlayingComponent.IsPlaying)
                {
                    AccessCallbacks(currentlyPlayingComponent).OnRewindCompleteAction.Unsubscribe(RewindCompletedComponents);
                    AccessCallbacks(currentlyPlayingComponent).OnRewindCompleteAction.Subscribe(RewindCompletedComponents);
                    currentlyPlayingComponent.Play(lastPlaybackDirection, lastPlaybackSpeedModifier);
                    return;
                }
                else
                {
                    AccessCallbacks(currentlyPlayingComponent).OnRewindCompleteAction.Unsubscribe(RewindCompletedComponents);
                    playingComponents.Remove(currentlyPlayingComponent);
                }
            }
            if (completedComponents.TryPop(out var queuedComponent))
            {
                try
                {
                    if (queuedComponent.Enabled)
                    {
                        AccessCallbacks(queuedComponent).OnRewindCompleteAction.Subscribe(RewindCompletedComponents);
                        queuedComponent.Play(lastPlaybackDirection, lastPlaybackSpeedModifier);
                        playingComponents.Add(queuedComponent);

                    }
                    else
                    {
                        RewindCompletedComponents();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                OnSequenceLoopCompleted();
            }
        }


        private void OnSequenceLoopCompleted()
        {
            switch (lastPlaybackDirection)
            {
                case PlaybackDirection.Forward:
                    OnLoopComplete();
                    break;
                default:
                    break;
            }
            for (int i = 0; i < playingComponents.Count; i++)
            {
                var component = playingComponents[i];
                if (component != null)
                {
                    if (component.TrackedHandle.IsActive())
                    {
                        component.TrackedHandle.TryCancel();
                    }
                }
            }
            isPlaying = false;
            playingComponents.Clear();
            if (startedLoopsCount > 0 && startedLoopsCount < settings.Loops)
            {
                //speed is already set, so we're passing 1 as playbackSpeedModifier
                Play(lastPlaybackDirection, 1);
                return;
            }
            switch (lastPlaybackDirection)
            {
                case PlaybackDirection.Forward:
                    OnComplete();
                    break;
                default:
                    OnRewindComplete();
                    break;
            }
        }

        private void OnLoopComplete()
        {
        }

        private void OnRewindComplete()
        {
            callbackActions.OnRewindCompleteAction.Invoke();
        }

        private void OnComplete()
        {
            callbackActions.OnCompleteAction.Invoke();
            OnCompleteEvent?.Invoke();
        }

        public sealed override void Stop()
        {
            CancellationTokenCleanUp();
            foreach (var component in components)
            {
                component?.Stop();
                AccessCallbacks(component).Clear();
            }
            isPlaying = false;
            startedLoopsCount = 0;
            longestComponent = null;
            playingComponents.Clear();
            waitingComponents.Clear();
            completedComponents.Clear();
        }
        public override void Pause()
        {
            foreach (var component in playingComponents)
            {
                component?.Pause();
            }
        }
        public override void Resume()
        {
            foreach (var component in playingComponents)
            {
                component?.Resume();
            }
        }
        public override void Rewind(float playbackSpeedModifier = 1f)
        {
            Play(PlaybackDirection.Backward, playbackSpeedModifier);
        }
    }
    public enum SequencePlayMode
    {
        Parallel = 0,
        Sequential = 1
    }
}
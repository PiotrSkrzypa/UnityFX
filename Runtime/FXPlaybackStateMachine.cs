using System.Collections.Generic;

namespace PSkrzypa.UnityFX
{
    public class FXPlaybackStateMachine
    {
        public FXPlaybackStateID CurrentState { get; private set; } = FXPlaybackStateID.Idle;

        private readonly HashSet<(FXPlaybackStateID from, FXPlaybackStateID to)> validTransitions = new();

        public FXPlaybackStateMachine()
        {
            AddDefaultTransitions();
        }

        private void AddDefaultTransitions()
        {
            Add(FXPlaybackStateID.Idle, FXPlaybackStateID.WaitingToStart);
            Add(FXPlaybackStateID.WaitingToStart, FXPlaybackStateID.Playing);
            Add(FXPlaybackStateID.Playing, FXPlaybackStateID.RepeatingDelay);
            Add(FXPlaybackStateID.RepeatingDelay, FXPlaybackStateID.Playing);
            Add(FXPlaybackStateID.Playing, FXPlaybackStateID.Completed);

            Add(FXPlaybackStateID.Cooldown, FXPlaybackStateID.Idle);
            Add(FXPlaybackStateID.Stopped, FXPlaybackStateID.Idle);
            Add(FXPlaybackStateID.Completed, FXPlaybackStateID.Idle);

            Add(FXPlaybackStateID.WaitingToStart, FXPlaybackStateID.Stopped);
            Add(FXPlaybackStateID.Playing, FXPlaybackStateID.Stopped);
            Add(FXPlaybackStateID.RepeatingDelay, FXPlaybackStateID.Stopped);
            Add(FXPlaybackStateID.Cooldown, FXPlaybackStateID.Stopped);

            Add(FXPlaybackStateID.WaitingToStart, FXPlaybackStateID.Cancelled);
            Add(FXPlaybackStateID.Playing, FXPlaybackStateID.Cancelled);
            Add(FXPlaybackStateID.RepeatingDelay, FXPlaybackStateID.Cancelled);
            Add(FXPlaybackStateID.Playing, FXPlaybackStateID.Cancelled);

            Add(FXPlaybackStateID.Cancelled, FXPlaybackStateID.Rewinding);
            Add(FXPlaybackStateID.Rewinding, FXPlaybackStateID.Completed);
            Add(FXPlaybackStateID.Rewinding, FXPlaybackStateID.RepeatingDelay);
            Add(FXPlaybackStateID.RepeatingDelay, FXPlaybackStateID.Rewinding);

        }

        private void Add(FXPlaybackStateID from, FXPlaybackStateID to)
        {
            validTransitions.Add((from, to));
        }

        public bool CanTransitionTo(FXPlaybackStateID to)
        {
            return validTransitions.Contains((CurrentState, to));
        }

        public bool TryTransitionTo(FXPlaybackStateID to)
        {
            if (!CanTransitionTo(to))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning($"[FX] Invalid state transition: {CurrentState} → {to}");
#endif
                return false;
            }

            CurrentState = to;
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"[FX] Transitioned to state: {to}");
#endif
            return true;
        }

        public void ForceSet(FXPlaybackStateID newState)
        {
            CurrentState = newState;
        }
    }
    public enum FXPlaybackStateID
    {
        Idle,
        WaitingToStart,
        Playing,
        RepeatingDelay,
        Cooldown,
        Completed,
        Stopped,
        Cancelled,
        Rewinding
    }
}

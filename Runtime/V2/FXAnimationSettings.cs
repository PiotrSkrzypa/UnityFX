using System;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    public record FXAnimationSettings
    {
        [SerializeField] float delay;
        [SerializeField] DelayType delayType;
        [SerializeField] float duration;
        [SerializeField] int loops;
        [SerializeField] LoopType loopType;
        [SerializeField] Ease ease;
        [SerializeField] bool timescaleIndependent;
        [SerializeField] float forwardPlaybackSpeed;
        [SerializeField] float backwardPlaybackSpeed;

        public FXAnimationSettings()
        {
            delay = 0f;
            delayType = DelayType.FirstLoop;
            duration = 1f;
            loops = 1;
            loopType = LoopType.Restart;
            ease = Ease.Linear;
            timescaleIndependent = false;
            forwardPlaybackSpeed = 1f;
            backwardPlaybackSpeed = 1f;
        }

        public float Delay { get => delay; }
        public DelayType DelayType { get => delayType; }
        public float Duration { get => duration; }
        public int Loops { get => loops; }
        public LoopType LoopType { get => loopType; }
        public Ease Ease { get => ease; }
        public bool TimescaleIndependent { get => timescaleIndependent; }
        public float ForwardPlaybackSpeed { get => forwardPlaybackSpeed; }
        public float BackwardPlaybackSpeed { get => backwardPlaybackSpeed; }
    }
}
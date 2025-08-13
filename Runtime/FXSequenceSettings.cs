using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public record FXSequenceSettings
    {
        [SerializeField] SequencePlayMode sequencePlayMode = SequencePlayMode.Parallel;
        [SerializeField] int loops = 1;
        [SerializeField] bool timescaleIndependent = false;
        [SerializeField] float forwardPlaybackSpeed = 1f;
        [SerializeField] float backwardPlaybackSpeed = 1f;

        public SequencePlayMode SequencePlayMode { get => sequencePlayMode; }
        public int Loops { get => loops; }
        public bool TimescaleIndependent { get => timescaleIndependent; }
        public float ForwardPlaybackSpeed { get => forwardPlaybackSpeed; }
        public float BackwardPlaybackSpeed { get => backwardPlaybackSpeed; }
    }
}
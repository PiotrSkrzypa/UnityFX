using System;
using Alchemy.Inspector;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXTiming
    {
        public float PlaybackSpeed = 1;
        public float InitialDelay;
        public float Duration;
        public float CooldownDuration;
        public bool ContributeToTotalDuration = true;

        public bool TimeScaleIndependent;

        public int NumberOfRepeats = 1;
        public bool RepeatForever;
        public float DelayBetweenRepeats;

        [ReadOnly]public int PlayCount;

        public IMotionScheduler GetScheduler() =>
    TimeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update;
    }
}

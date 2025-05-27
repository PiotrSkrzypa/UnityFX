using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXSequenceTiming : FXTiming
    {
        [SerializeField] SequencePlayMode playMode = SequencePlayMode.Parallel;
        public SequencePlayMode PlayMode { get => playMode; }
        [SerializeField]private float computedDuration;

        public float ComputedDuration { get => computedDuration; }

        public void RecalculateDuration(IFXComponent[] components)
        {
            float result = 0f;

            foreach (var comp in components)
            {
                if (comp == null) continue;
                var timing = comp.Timing;
                if (timing == null) continue;

                float compDuration = timing.InitialDelay + timing.Duration;

                if (timing.RepeatForever)
                {
                    compDuration += timing.DelayBetweenRepeats;
                }
                else
                {
                    compDuration = ( compDuration + timing.DelayBetweenRepeats ) * Mathf.Max(1, timing.NumberOfRepeats);
                }

                switch (playMode)
                {
                    case SequencePlayMode.Parallel:
                        result = Mathf.Max(result, compDuration);
                        break;
                    case SequencePlayMode.Sequential:
                        result += compDuration;
                        break;
                }
            }

            if (RepeatForever)
            {
                computedDuration = float.PositiveInfinity;
                Duration = computedDuration;
                return;
            }

            float loop = (result + DelayBetweenRepeats) * Mathf.Max(1, NumberOfRepeats);
            computedDuration = InitialDelay + loop + CooldownDuration;
            Duration = computedDuration;
        }
    } 
}

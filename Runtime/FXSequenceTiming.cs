using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXSequenceTiming : FXTiming
    {
        [SerializeField]private float computedDuration;

        public float ComputedDuration { get => computedDuration; }

        public void RecalculateDuration(IFXComponent[] components, SequencePlayMode mode)
        {
            float result = 0f;

            foreach (var comp in components)
            {
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

                switch (mode)
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

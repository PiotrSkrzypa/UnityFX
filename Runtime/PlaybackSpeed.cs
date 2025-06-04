

using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [System.Serializable]
    public struct PlaybackSpeed
    {
        public float speed;
        public float rewindSpeed;
        public float SpeedAbs
        {
            get
            {
                if (!initialized)
                {
                    speedAbs = Mathf.Abs(speed);
                }
                return speedAbs;
            }
            private set => speedAbs = value;
        }
        public float RewindSpeedAbs
        {
            get
            {
                if (!initialized)
                {
                    rewindSpeedAbs = Mathf.Abs(rewindSpeed);
                }
                return rewindSpeedAbs;
            }
            private set => rewindSpeedAbs = value;
        }
        bool initialized;
        float speedAbs;
        float rewindSpeedAbs;

        public PlaybackSpeed(float speed, float rewindSpeed)
        {
            this.speed = speed;
            this.rewindSpeed = rewindSpeed;
            initialized = true;
            speedAbs = Mathf.Abs(speed);
            rewindSpeedAbs = Mathf.Abs(rewindSpeed);
            SpeedAbs = speedAbs;
            RewindSpeedAbs = rewindSpeedAbs;
        }


        public PlaybackSpeed Flip()
        {
            return new PlaybackSpeed(rewindSpeed, speed);
        }
        public static PlaybackSpeed operator *(PlaybackSpeed playbackSpeedA, PlaybackSpeed playbackSpeedB)
        {
            return new PlaybackSpeed(playbackSpeedA.speed * playbackSpeedB.speed, playbackSpeedA.rewindSpeed * playbackSpeedB.rewindSpeed);
        }
    }
}

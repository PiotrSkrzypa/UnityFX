

using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [System.Serializable]
    public struct PlaybackSpeed
    {
        public float speed;
        public float rewindSpeed;
        public float SpeedAbs { get; private set; }
        public float RewindSpeedAbs { get; private set; }
        
        public PlaybackSpeed(float speed, float rewindSpeed)
        {
            this.speed = speed;
            this.rewindSpeed = rewindSpeed;
            SpeedAbs = Mathf.Abs(speed);
            RewindSpeedAbs = Mathf.Abs(rewindSpeed);
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

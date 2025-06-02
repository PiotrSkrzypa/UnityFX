

namespace PSkrzypa.UnityFX
{
    [System.Serializable]
    public struct PlaybackSpeed
    {
        public float speed;
        public float rewindSpeed;
        public PlaybackSpeed(float speed, float rewindSpeed)
        {
            this.speed = speed;
            this.rewindSpeed = rewindSpeed;
        }
        public PlaybackSpeed Flip()
        {
            return new PlaybackSpeed(rewindSpeed, speed);
        }
    }
}

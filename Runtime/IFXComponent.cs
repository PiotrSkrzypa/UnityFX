using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    public interface IFXComponent
    {
        float Progress { get; }
        FXTiming Timing { get; }
        bool CanPlayWhenAlreadyPlaying { get; }
        bool RewindOnCancel { get; }
        float RewindSpeedMultiplier { get; }
        FXPlaybackStateID CurrentState { get; }
        void Initialize();
        UniTask Play(PlaybackSpeed playbackSpeed);
        void Reset();
        void Stop();
        void Cancel();
    }
}
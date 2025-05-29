using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    public interface IFXComponent
    {
        FXTiming Timing { get; }
        bool CanPlayWhenAlreadyPlaying { get; }
        bool ReverseOnCancel { get; }
        float ReverseSpeedMultiplier { get; }
        FXPlaybackStateID CurrentState { get; }
        void Initialize();
        UniTask Play(float inheritedSpeed = 1f);
        void Reset();
        void Stop();
        void Cancel();
    }
}
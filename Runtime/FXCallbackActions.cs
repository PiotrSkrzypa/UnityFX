using System;

namespace PSkrzypa.UnityFX
{
    public class FXCallbackActions
    {
        FXCallback onRewindCompleteAction = new FXCallback();
        FXCallback onCancelAction = new FXCallback();
        FXCallback onCompleteAction = new FXCallback();

        public FXCallback OnRewindCompleteAction { get => onRewindCompleteAction; }
        public FXCallback OnCancelAction { get => onCancelAction; }
        public FXCallback OnCompleteAction { get => onCompleteAction; }

        public void Clear()
        {
            onRewindCompleteAction.Clear();
            onCancelAction.Clear();
            onCompleteAction.Clear();
        }

    }
    public class FXCallback
    {
        Action action;
        public void Invoke()
        {
            action?.Invoke();
        }
        public void Subscribe(Action action)
        {
            this.action += action;
        }
        public void Unsubscribe(Action action)
        {
            this.action -= action;
        }
        public void Clear()
        {
            action = null;
        }
    }

}
using Core.Input.Interfaces;
using UnityEngine;

namespace Core.Input.Providers
{
    /// <summary>
    /// Base class that holds reference to <see cref="PlayerInputActions"/> and handles lifecycle.
    /// Note: we no longer try to *implement* generic parameter – derived classes do that.
    /// </summary>
    public abstract class InputProviderBase<TInterface> : MonoBehaviour, IInputProvider where TInterface : class
    {
        protected PlayerInputActions input;

        public void Initialize(PlayerInputActions actions)
        {
            input = actions;
            RegisterCallbacks();
        }

        protected virtual void OnDestroy() => UnregisterCallbacks();
        protected abstract void RegisterCallbacks();
        protected abstract void UnregisterCallbacks();
    }
}
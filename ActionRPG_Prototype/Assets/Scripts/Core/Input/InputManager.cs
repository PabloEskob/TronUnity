using Core.Input.Interfaces;
using Core.Input.Providers;
using Core.Services;
using UnityEngine;

namespace Core.Input
{
    [AddComponentMenu("Pavel/Input Manager")]
    [DisallowMultipleComponent]
    public sealed class InputManager : MonoBehaviour
    {
        public PlayerInputActions Actions { get; private set; }

        public IMovementInput Movement { get; private set; }
        public ICameraInput Camera { get; private set; }
        public ICombatInput Combat { get; private set; }

        private void Awake()
        {
            // Если уже есть зарегистрированный экземпляр, уничтожаем дубликат
            if (Core.Services.ServiceLocator.TryGet<InputManager>(out var existing) && existing != this)
            {
                Destroy(gameObject);
                return;
            }

            Actions = new PlayerInputActions();
            Actions.Enable();
            
            Movement = gameObject.AddComponent<Providers.MovementInputProvider>();
            Camera   = gameObject.AddComponent<Providers.CameraInputProvider>();
            Combat   = gameObject.AddComponent<Providers.CombatInputProvider>();

            foreach (var p in GetComponents<Core.Input.Interfaces.IInputProvider>())
                p.Initialize(Actions);

            Core.Services.ServiceLocator.Register(this);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            // Отписываемся, только если этот экземпляр был зарегистрирован
            if (Core.Services.ServiceLocator.TryGet<InputManager>(out var existing) && existing == this)
                Core.Services.ServiceLocator.Unregister<InputManager>();

            Actions?.Dispose();
        }
    }
}
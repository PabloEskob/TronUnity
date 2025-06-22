using Core.Input.Interfaces;
using Core.Input.Providers;
using Core.Services;
using UnityEngine;

namespace Core.Input
{
    public class InputManager : MonoBehaviour
    {
        private PlayerInputActions _inputActions;

        // Public properties
        public IMovementInput Movement { get; private set; }
        public ICameraInput Camera { get; private set; }
        public ICombatInput Combat { get; private set; }

        private void Awake()
        {
            // Создаем Input Actions
            _inputActions = new PlayerInputActions();

            // Создаем GameObject для провайдеров
            var providersContainer = new GameObject("InputProviders");
            providersContainer.transform.SetParent(transform);

            // Создаем и инициализируем провайдеры
            Movement = CreateProvider<MovementInputProvider>(providersContainer, "MovementProvider");
            Camera = CreateProvider<CameraInputProvider>(providersContainer, "CameraProvider");
            Combat = CreateProvider<CombatInputProvider>(providersContainer, "CombatProvider");

            // Регистрируем сервис
            ServiceLocator.Instance.RegisterService<InputManager>(this);

            // Сохраняем между сценами
            DontDestroyOnLoad(gameObject);
        }

        private T CreateProvider<T>(GameObject parent, string name) where T : MonoBehaviour
        {
            var providerGO = new GameObject(name);
            providerGO.transform.SetParent(parent.transform);
            var provider = providerGO.AddComponent<T>();

            // Если провайдер имеет метод Initialize, вызываем его
            if (provider is IInputProvider inputProvider)
            {
                inputProvider.Initialize(_inputActions);
            }

            return provider;
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.UnregisterService<InputManager>();
            _inputActions?.Dispose();
        }
    }
}
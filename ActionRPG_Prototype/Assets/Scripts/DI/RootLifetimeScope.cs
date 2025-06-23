using Character.Movement;
using Config.Movement;
using Core.Events;
using Core.Input;
using Core.Input.Interfaces;
using Core.Interfaces;
using Core.Spawn;
using Effect;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfiguration _gameConfig;
        [SerializeField] private MovementConfig _movementConfig;
        [SerializeField] private Transform _playerSpawnPoint;

        protected override void Configure(IContainerBuilder builder)
        {
            // Конфиги
            builder.RegisterInstance(_gameConfig);
            builder.RegisterInstance(_movementConfig);
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Сервисы
            builder.Register<IGameSettings, GameSettingsService>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<InputManager>().As<IMovementInput>();
            

            // PlayerSpawner с явным указанием параметров
            builder.Register<IPlayerSpawner>(container => new PlayerSpawner(
                container.Resolve<GameConfiguration>(),
                _playerSpawnPoint, // Напрямую передаем Transform
                container
            ), Lifetime.Singleton);

            builder.RegisterEntryPoint<GameSettingsApplier>();
            builder.RegisterEntryPoint<GameInitializer>();
        }
    }
}
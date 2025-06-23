using Config.Movement;
using Core.Camera;
using Core.Events;
using Core.Input;
using Core.Input.Interfaces;
using Core.Interfaces;
using Core.Spawn;
using Unity.Cinemachine;
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
            // ─── Конфиги ───────────────────────────────────────────
            builder.RegisterInstance(_gameConfig);
            builder.RegisterInstance(_movementConfig);
            builder.RegisterInstance(_playerSpawnPoint);

            // ─── Инфраструктура ───────────────────────────────────
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // ─── InputManager (уже есть в сцене) ───────────────────
            builder.RegisterComponentInHierarchy<InputManager>()
                .As<IMovementInput, ICameraInput, ICombatInput>();

            // ─── Камера и её компоненты ────────────────────────────
            builder.RegisterComponentInHierarchy<CinemachineCamera>();

            // ─── Геймплей сервисы ──────────────────────────────────
            builder.Register<IGameSettings, GameSettingsService>(Lifetime.Singleton);
            builder.Register<IPlayerSpawner, PlayerSpawner>(Lifetime.Singleton);

            // ─── Entry-points ───────────────────────────────────────
            builder.RegisterEntryPoint<GameSettingsApplier>();
            builder.RegisterEntryPoint<CinemachinePlayerBinder>(); // прилипание Follow/LookAt
            builder.RegisterEntryPoint<GameInitializer>();
           
        }
    }
}
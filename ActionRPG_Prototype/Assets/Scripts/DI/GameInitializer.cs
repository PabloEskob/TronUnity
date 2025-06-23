using Core.Input.Interfaces;
using System;
using Core.Interfaces;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    /// <summary>
    /// Оркестр старта игры: спавнит игрока и применяет настройки.
    /// </summary>
    public class GameInitializer : IStartable, IDisposable
    {
        private readonly IPlayerSpawner _spawner;
        private readonly IGameSettingsApplier _settingsApplier;
        private GameObject _playerInstance;

        [Inject]
        public GameInitializer(IPlayerSpawner spawner,
            IGameSettingsApplier settingsApplier)
        {
            _spawner = spawner;
            _settingsApplier = settingsApplier;
        }

        public void Start()
        {
            _playerInstance = _spawner.SpawnPlayer();
            _settingsApplier.Apply();
            Debug.Log("Game initialized ✨");
        }

        public void Dispose()
        {
            if (_playerInstance != null)
            {
                _spawner.DespawnPlayer(_playerInstance);
                _playerInstance = null;
            }
        }
    }
}
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
        private GameObject _playerInstance;
        
        [Inject]
        public GameInitializer(IPlayerSpawner spawner) => _spawner = spawner;

        public void Start()
        {
            _playerInstance = _spawner.SpawnPlayer();
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
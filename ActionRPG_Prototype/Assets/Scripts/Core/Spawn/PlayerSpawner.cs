using Core.Interfaces;
using DI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Spawn
{
    public class PlayerSpawner : IPlayerSpawner
    {
        private readonly GameObject _prefab;
        private readonly Transform _spawnPoint;
        private readonly IObjectResolver _container;

        public PlayerSpawner(
            GameConfiguration cfg,
            Transform spawnPoint, // Без атрибута
            IObjectResolver container)
        {
            _prefab = cfg.Player.Prefab;
            _spawnPoint = spawnPoint;
            _container = container;
        }

        public GameObject SpawnPlayer()
            => _container.Instantiate(_prefab, _spawnPoint.position, Quaternion.identity);

        public void DespawnPlayer(GameObject player)
            => Object.Destroy(player);
    }
}
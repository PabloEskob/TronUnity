using Core.Events; // IEventBus
using Core.Events.Messages; // PlayerSpawned
using Core.Interfaces;
using DI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Spawn
{
    public sealed class PlayerSpawner : IPlayerSpawner
    {
        readonly GameObject _prefab;
        readonly Transform _spawnPoint;
        readonly IObjectResolver _container;
        readonly IEventBus _bus;

        [Inject] // вызывается контейнером
        public PlayerSpawner(
            GameConfiguration cfg,
            Transform spawnPoint,
            IObjectResolver container,
            IEventBus bus) // ← добавлен параметр
        {
            _prefab = cfg.Player.Prefab;
            _spawnPoint = spawnPoint;
            _container = container;
            _bus = bus;
        }

        public GameObject SpawnPlayer()
        {
            // Через VContainer.Instantiate, чтобы у компонентов префаба сработали [Inject]
            var player = _container.Instantiate(
                _prefab,
                _spawnPoint.position,
                Quaternion.identity,
                null);

            _bus.Publish(new PlayerSpawned(player)); // событие в шину
            return player;
        }

        public void DespawnPlayer(GameObject player)
        {
            if (player != null)
                Object.Destroy(player);
        }
    }
}
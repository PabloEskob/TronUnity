using System;
using System.Collections.Generic;
using Core.Scripts.AssetManagement;
using Core.Scripts.Services.PersistentProgress;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Infrastructure.States.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assets;
        private readonly IObjectResolver _container;

        public List<ISavedProgressReader> ProgressReaders { get; } = new();
        public List<ISavedProgress> ProgressWriters { get; } = new();
        
        public GameObject HeroGameObject { get; private set; }
        public event Action HeroCreated;
        
        public GameFactory(IAssetProvider assets, IObjectResolver container)
        {
            _assets = assets;
            _container = container;
        }

        public GameObject CreateHero(GameObject at)
        {
            HeroGameObject = InstantiateRegistered(AssetPath.HeroPlayerPath, at.transform.position);
            _container.InjectGameObject(HeroGameObject);
            HeroCreated?.Invoke();
            return HeroGameObject;
        }

        public void Cleanup()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
        }
        
        private GameObject InstantiateRegistered(string prefabPath, Vector3 at)
        {
            var gameObject = _assets.Instantiate(prefabPath, at);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateRegistered(string prefabPath)
        {
            var gameObject = _assets.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private void RegisterProgressWatchers(GameObject hero)
        {
            foreach (var progressReader in hero.GetComponentsInChildren<ISavedProgressReader>())
                Register(progressReader);
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
                ProgressWriters.Add(progressWriter);

            ProgressReaders.Add(progressReader);
        }
    }
}
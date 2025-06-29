using Core.Scripts.AssetManagement;
using Core.Scripts.CameraLogic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Infrastructure.States.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assets;
        
        private readonly IObjectResolver _container;
        
        public GameFactory(IAssetProvider assets, IObjectResolver container)
        {
            _assets = assets;
            _container = container;
        }

        public GameObject CreateHero(GameObject at)
        {
            var hero = _assets.Instantiate(AssetPath.HeroPlayerPath, at.transform.position);
            _container.InjectGameObject(hero);
            return hero;
        }

        public void CreateCamera(GameObject hero)
        {
            _assets.Instantiate(AssetPath.CameraPath).GetComponent<CameraFollow>().Follow(hero);
        }
    }
}
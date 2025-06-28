using Core.Scripts.AssetManagement;
using Core.Scripts.CameraLogic;
using UnityEngine;

namespace Core.Scripts.Infrastructure
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assets;

        public GameFactory(IAssetProvider assets) =>
            _assets = assets;

        public GameObject CreateHero(GameObject at) =>
            _assets.Instantiate(AssetPath.HeroPlayerPath, at.transform.position);

        public void CreateCamera(GameObject hero) =>
            _assets.Instantiate(AssetPath.CameraPath).GetComponent<CameraFollow>().Follow(hero);
    }
}
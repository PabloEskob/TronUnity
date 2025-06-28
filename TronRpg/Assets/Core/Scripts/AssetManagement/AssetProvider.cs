using UnityEngine;

namespace Core.Scripts.AssetManagement
{
    public class AssetProvider: IAssetProvider
    {
        public GameObject Instantiate(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }

        public GameObject Instantiate(string path, Vector3 at)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, at, Quaternion.identity);
        }

        public GameObject LoadAsset(string path)
        {
            throw new System.NotImplementedException();
        }

        public T LoadAsset<T>(string path) where T : Component
        {
            throw new System.NotImplementedException();
        }
    }
}
using UnityEngine;

namespace Core.Scripts.AssetManagement
{
    public interface IAssetProvider
    {
        GameObject LoadAsset(string path);
        T LoadAsset<T>(string path) where T : Component;

        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 at);
    }
}
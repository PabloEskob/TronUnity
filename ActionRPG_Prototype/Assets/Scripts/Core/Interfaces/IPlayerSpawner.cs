using UnityEngine;

namespace Core.Interfaces
{
    public interface IPlayerSpawner
    {
        GameObject SpawnPlayer();
        void DespawnPlayer(GameObject player);
    }
}
using UnityEngine;

namespace Core.Scripts.Infrastructure
{
    public interface IGameFactory
    {
        GameObject CreateHero(GameObject at);
        void CreateCamera(GameObject hero);
    }
}
using UnityEngine;

namespace Core.Scripts.Infrastructure.States.Factory
{
    public interface IGameFactory
    {
        GameObject CreateHero(GameObject at);
        void CreateCamera(GameObject hero);
    }
}
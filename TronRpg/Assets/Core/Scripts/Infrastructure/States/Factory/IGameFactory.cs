using System.Collections.Generic;
using Core.Scripts.Services.PersistentProgress;
using UnityEngine;

namespace Core.Scripts.Infrastructure.States.Factory
{
    public interface IGameFactory
    {
        GameObject CreateHero(GameObject at);
        void CreateCamera(GameObject hero);
        void Cleanup();
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
    }
}
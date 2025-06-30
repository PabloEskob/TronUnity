using Core.Scripts.Data;
using Core.Scripts.Infrastructure.States.Factory;
using Core.Scripts.Services.PersistentProgress;
using UnityEngine;

namespace Core.Scripts.Services.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string ProgressKey = "Progress";
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;

        public SaveLoadService(IGameFactory gameFactory,IPersistentProgressService progressService)
        {
            _gameFactory = gameFactory;
            _progressService = progressService;
        }

        public void SaveProgress()
        {
            foreach (var progressWriter in _gameFactory.ProgressWriters) 
                progressWriter.UpdateProgress(_progressService.Progress);
            
            PlayerPrefs.SetString(ProgressKey, _progressService.Progress.ToJson());
        }

        public PlayerProgress LoadProgress() => 
            PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<PlayerProgress>();
    }
}
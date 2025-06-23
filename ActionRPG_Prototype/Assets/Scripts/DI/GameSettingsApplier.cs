using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class GameSettingsApplier : IStartable, IDisposable
    {
        private readonly IGameSettings _settings;

        [Inject]
        public GameSettingsApplier(IGameSettings settings) => _settings = settings;

        public void Start()
        {
            Apply(); // первый запуск
            _settings.OnSettingsChanged += Apply;
        }

        public void Dispose() => _settings.OnSettingsChanged -= Apply;

        private void Apply()
        {
            AudioListener.volume = _settings.MasterVolume;
            QualitySettings.vSyncCount = _settings.VSync ? 1 : 0;
            QualitySettings.SetQualityLevel(_settings.GraphicsQuality);
        }
    }
}
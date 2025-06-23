using Core.Interfaces;
using UnityEngine;
using VContainer;

namespace DI
{
    public class GameSettingsApplier : IGameSettingsApplier
    {
        private readonly IGameSettings _settings;

        [Inject]
        public GameSettingsApplier(IGameSettings settings) => _settings = settings;

        public void Apply()
        {
            AudioListener.volume = _settings.MasterVolume;
            QualitySettings.vSyncCount = _settings.VSync ? 1 : 0;
            QualitySettings.SetQualityLevel(_settings.GraphicsQuality);
        }
    }
}
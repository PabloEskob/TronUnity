using UnityEngine;
using VContainer;

namespace DI
{
    public interface IGameSettings
    {
        float MasterVolume { get; }
        int GraphicsQuality { get; }
        bool VSync { get; }
        event System.Action OnSettingsChanged;
    }

    public class GameSettingsService : IGameSettings
    {
        public float MasterVolume { get; private set; }
        public int GraphicsQuality { get; private set; }
        public bool VSync { get; private set; }

        public event System.Action OnSettingsChanged;

        private const string VolumeKey = "volume";
        private const string QualityKey = "quality";
        private const string VSyncKey = "vsync";

        [Inject]
        public GameSettingsService()
        {
            Load();
        }

        public void ApplySettings(float volume, int quality, bool vsync)
        {
            MasterVolume = Mathf.Clamp01(volume);
            GraphicsQuality = Mathf.Clamp(quality, 0, QualitySettings.names.Length - 1);
            VSync = vsync;

            AudioListener.volume = MasterVolume;
            QualitySettings.SetQualityLevel(GraphicsQuality);
            QualitySettings.vSyncCount = VSync ? 1 : 0;

            Save();
            OnSettingsChanged?.Invoke();
        }

        private void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
            GraphicsQuality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
            VSync = PlayerPrefs.GetInt(VSyncKey, QualitySettings.vSyncCount) == 1;
        }

        private void Save()
        {
            PlayerPrefs.SetFloat(VolumeKey, MasterVolume);
            PlayerPrefs.SetInt(QualityKey, GraphicsQuality);
            PlayerPrefs.SetInt(VSyncKey, VSync ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
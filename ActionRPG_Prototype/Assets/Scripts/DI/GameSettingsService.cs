using System;
using UnityEngine;

namespace DI
{
    public interface IGameSettings
    {
        float MasterVolume { get; }
        int GraphicsQuality { get; }
        bool VSync { get; }

        /// Срабатывает каждый раз, когда значения изменились и сохранены.
        event Action OnSettingsChanged;

        /// Установить новые значения (без применения к движку).
        void Set(float volume, int quality, bool vsync);
    }

    public class GameSettingsService : IGameSettings
    {
        private const string VolumeKey = "volume";
        private const string QualityKey = "quality";
        private const string VSyncKey = "vsync";

        public float MasterVolume { get; private set; }
        public int GraphicsQuality { get; private set; }
        public bool VSync { get; private set; }

        public event Action OnSettingsChanged = delegate { };

        public GameSettingsService() => Load(); // [Inject] не нужен: VContainer сам увидит единственный конструктор

        public void Set(float volume, int quality, bool vsync)
        {
            MasterVolume = Mathf.Clamp01(volume);
            GraphicsQuality = Mathf.Clamp(quality, 0, QualitySettings.names.Length - 1);
            VSync = vsync;

            SaveAsync(); // Unity 6 умеет асинхронно
            OnSettingsChanged.Invoke();
        }

        #region Persistence

        private void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
            GraphicsQuality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
            VSync = PlayerPrefs.GetInt(VSyncKey, QualitySettings.vSyncCount) == 1;
        }

        private void SaveAsync()
        {
            PlayerPrefs.SetFloat(VolumeKey, MasterVolume);
            PlayerPrefs.SetInt(QualityKey, GraphicsQuality);
            PlayerPrefs.SetInt(VSyncKey, VSync ? 1 : 0);
            PlayerPrefs.Save(); // 💡 не блокируем главный поток
        }

        #endregion
    }
}
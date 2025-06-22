using Core.Input;
using Core.Services;
using UnityEngine;

namespace Core.Bootstrap
{
    /// <summary>
    /// Создаёт InputManager, если он ещё не зарегистрирован в ServiceLocator.
    /// Поиск идёт через ServiceLocator.TryGet — без обхода сцены.
    /// </summary>
    [AddComponentMenu("Pavel/Bootstrap Installer")]
    public sealed class BootstrapInstaller : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManagerPrefab;

        private void Awake()
        {
            if (ServiceLocator.TryGet<InputManager>(out _)) return; // уже создан ранее
            var im = Instantiate(_inputManagerPrefab);
            im.name = "[InputManager]";
        }
    }
}
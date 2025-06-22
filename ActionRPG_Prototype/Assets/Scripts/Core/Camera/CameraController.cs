using Core.Camera.Effects;
using Core.Input.Interfaces;
using Core.Services;
using UnityEngine;
using Config.Camera;
using Core.Input;

namespace Core.Camera
{
    [RequireComponent(typeof(CameraStateManager))]
    public class CameraController : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private CameraConfig _config;

        [Header("Effects")] [SerializeField] private CameraShakeEffect _shakeEffect;
        [SerializeField] private CameraFOVEffect _fovEffect;

        private ICameraInput _input;
        private UnityEngine.Camera _camera;
        private CameraStateManager _stateManager;

        public ICameraInput Input => _input;
        public CameraConfig ActiveConfig { get; private set; }

        private void Awake()
        {
            _camera = GetComponentInChildren<UnityEngine.Camera>();
            _stateManager = GetComponent<CameraStateManager>();
            _shakeEffect = GetComponent<CameraShakeEffect>();
            _fovEffect = GetComponent<CameraFOVEffect>();

            ActiveConfig = _config;
        }

        private void Start()
        {
            var inputManager = ServiceLocator.Instance.GetService<InputManager>();
            _input = inputManager.Camera;

            ServiceLocator.Instance.RegisterService<CameraController>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.UnregisterService<CameraController>();
        }

        public void SetConfig(CameraStateConfig stateConfig)
        {
            // Apply state-specific configuration
            if (stateConfig != null)
            {
                _camera.fieldOfView = stateConfig.fieldOfView;
                // Apply other state-specific settings
            }
        }

        public void Shake(float intensity, float duration)
        {
            _shakeEffect?.TriggerShake(intensity, duration);
        }

        public void SetFOV(float targetFOV, float duration)
        {
            _fovEffect?.SetFOV(targetFOV, duration);
        }

        public void ResetFOV(float duration)
        {
            _fovEffect?.ResetFOV(duration);
        }
    }
}
// CameraEffectsController.cs - Fixed version

using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using Movement.Interface;

namespace Camera
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraEffectsController : MonoBehaviour
    {
        [Header("References")] [Tooltip("The main virtual camera whose FOV will be affected by speed.")] [SerializeField]
        private CinemachineCamera _mainVirtualCamera;

        [Tooltip("Component that provides velocity information. Must implement IVelocityProvider (e.g., CharacterMovementController).")]
        [SerializeField]
        private MonoBehaviour _velocityProviderComponent; // Принимаем MonoBehaviour для удобства назначения в инспекторе

        private IVelocityProvider _velocityProvider;

        private CinemachineImpulseSource _impulseSource;

        [Header("Screen Shake Settings")] [SerializeField]
        private float _lightShakeForce = 0.5f;

        [SerializeField] private float _mediumShakeForce = 1f;
        [SerializeField] private float _heavyShakeForce = 2f;

        [Tooltip("Default interval in seconds between impulses for continuous shake.")] [SerializeField]
        private float _continuousShakeInterval = 0.1f;

        [Header("Speed Effects (FOV) Settings")] [SerializeField]
        private bool _enableSpeedEffects = true;

        [Tooltip("Speed threshold to start applying FOV effects.")] [SerializeField]
        private float _speedThreshold = 10f;

        [Tooltip("Speed at which the FOV effect reaches its maximum.")] [SerializeField]
        private float _maxSpeedForMaxFOV = 20f;

        [Tooltip("Maximum Field of View when at or above Max Speed for Max FOV.")] [SerializeField]
        private float _maxSpeedFOV = 70f;

        [Tooltip("Normal Field of View when below speed threshold or effects are off.")] [SerializeField]
        private float _normalFOV = 60f;

        [Tooltip("How quickly the FOV adapts to speed changes (higher value = faster adaptation).")] [SerializeField]
        private float _speedFovAdaptationSpeed = 5f;

        [Tooltip(
            "How quickly the FOV adapts during explicit SetFieldOfView calls (higher value = faster adaptation for Lerp). Not used if duration is > 0 in SetFieldOfView.")]
        [SerializeField]
        private float _explicitFovChangeSpeed = 8f;


        private Coroutine _currentShakeCoroutine;
        private Coroutine _fovTransitionCoroutine; // Корутина для явного изменения FOV через SetFieldOfView

        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();

            if (_mainVirtualCamera == null)
            {
                UnityEngine.Debug.LogWarning("CameraEffectsController: Main Virtual Camera is not assigned. FOV effects will not work.", this);
            }

            SetupVelocityProvider();
        }

        private void SetupVelocityProvider()
        {
            if (_velocityProviderComponent != null)
            {
                _velocityProvider = _velocityProviderComponent as IVelocityProvider;
                if (_velocityProvider == null)
                {
                    UnityEngine.Debug.LogError(
                        $"CameraEffectsController: Assigned Velocity Provider Component ('{_velocityProviderComponent.gameObject.name}') does not implement IVelocityProvider.",
                        this);
                }
            }
            else
            {
                if (_enableSpeedEffects) // Предупреждаем, только если эффекты скорости включены
                {
                    UnityEngine.Debug.LogWarning(
                        "CameraEffectsController: Velocity Provider Component is not assigned. Speed-based FOV effects will be disabled.", this);
                }
            }
        }

        private void Update()
        {
            // Применяем эффект скорости, только если он включен, есть провайдер, он доступен, есть камера,
            // и не идет активная корутина явного изменения FOV.
            if (_enableSpeedEffects &&
                _velocityProvider != null &&
                _velocityProvider.IsAvailable &&
                _mainVirtualCamera != null &&
                _fovTransitionCoroutine == null) // Не мешаем явной установке FOV
            {
                ApplySpeedBasedFOV();
            }
        }

        private void ApplySpeedBasedFOV()
        {
            float currentSpeed = _velocityProvider.Velocity.magnitude;
            float targetFOV;

            if (currentSpeed > _speedThreshold)
            {
                float speedRatio = Mathf.InverseLerp(_speedThreshold, _maxSpeedForMaxFOV, currentSpeed);
                targetFOV = Mathf.Lerp(_normalFOV, _maxSpeedFOV, speedRatio);
            }
            else
            {
                targetFOV = _normalFOV;
            }

            var lens = _mainVirtualCamera.Lens;
            if (!Mathf.Approximately(lens.FieldOfView, targetFOV)) // Изменяем, только если есть разница
            {
                lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, Time.deltaTime * _speedFovAdaptationSpeed);
                _mainVirtualCamera.Lens = lens;
            }
        }

        public void TriggerShake(ShakeIntensity intensity)
        {
            if (!isActiveAndEnabled || _impulseSource == null) return;

            float force = intensity switch
            {
                ShakeIntensity.Light => _lightShakeForce,
                ShakeIntensity.Medium => _mediumShakeForce,
                ShakeIntensity.Heavy => _heavyShakeForce,
                _ => _lightShakeForce
            };
            _impulseSource.GenerateImpulse(force);
        }

        public void TriggerDirectionalShake(Vector3 direction, float force)
        {
            if (!isActiveAndEnabled || _impulseSource == null) return;

            // Сохраняем и восстанавливаем, только если DefaultVelocity не нулевая,
            // иначе это может быть намеренная настройка для этого импульса.
            // Однако, более безопасный подход - всегда сохранять и восстанавливать.
            Vector3 originalDefaultVelocity = _impulseSource.DefaultVelocity;
            _impulseSource.DefaultVelocity = direction.normalized * force;
            _impulseSource.GenerateImpulse();
            _impulseSource.DefaultVelocity = originalDefaultVelocity;
        }

        public void StartContinuousShake(float intensity, float duration, float? interval = null)
        {
            if (!isActiveAndEnabled || _impulseSource == null) return;

            if (_currentShakeCoroutine != null)
            {
                StopCoroutine(_currentShakeCoroutine);
            }

            _currentShakeCoroutine = StartCoroutine(ContinuousShakeCoroutine(intensity, duration, interval ?? _continuousShakeInterval));
        }

        public void StopContinuousShake()
        {
            if (_currentShakeCoroutine != null)
            {
                StopCoroutine(_currentShakeCoroutine);
                _currentShakeCoroutine = null;
            }
        }

        private IEnumerator ContinuousShakeCoroutine(float intensity, float duration, float interval)
        {
            if (interval <= 0) // Защита от бесконечного цикла, если интервал некорректен
            {
                UnityEngine.Debug.LogWarning("ContinuousShakeCoroutine: Interval must be greater than zero. Aborting shake.", this);
                _currentShakeCoroutine = null;
                yield break;
            }

            float elapsed = 0;
            // Кэшируем WaitForSeconds для производительности, если интервал не меняется часто.
            // Если интервал может быть разным для каждого вызова, можно создавать его внутри.
            var waitInstruction = new WaitForSeconds(interval);

            while (elapsed < duration)
            {
                _impulseSource.GenerateImpulse(intensity);
                elapsed += interval;
                yield return waitInstruction;
            }

            _currentShakeCoroutine = null;
        }

        /// <summary>
        /// Smoothly transitions the Field of View of the main virtual camera.
        /// </summary>
        /// <param name="targetFOV">The desired Field of View.</param>
        /// <param name="duration">Duration of the transition in seconds. If 0 or less, FOV is set instantly.</param>
        public void SetFieldOfView(float targetFOV, float duration)
        {
            if (!isActiveAndEnabled || _mainVirtualCamera == null)
            {
                if (_mainVirtualCamera == null)
                    UnityEngine.Debug.LogWarning("CameraEffectsController: Cannot set Field Of View, Main Virtual Camera is not assigned.", this);
                return;
            }

            if (_fovTransitionCoroutine != null)
            {
                StopCoroutine(_fovTransitionCoroutine);
            }

            if (duration <= 0f)
            {
                var lensInsta = _mainVirtualCamera.Lens;
                lensInsta.FieldOfView = targetFOV;
                _mainVirtualCamera.Lens = lensInsta;
                _fovTransitionCoroutine = null; // Убедимся, что сброшен
            }
            else
            {
                _fovTransitionCoroutine = StartCoroutine(FOVTransitionCoroutine(targetFOV, duration));
            }
        }

        private IEnumerator FOVTransitionCoroutine(float targetFOV, float duration)
        {
            var lens = _mainVirtualCamera.Lens;
            float startFOV = lens.FieldOfView;
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration); // Используем Clamp01 для корректного t
                lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t); // Прямой Lerp по времени для заданной длительности
                _mainVirtualCamera.Lens = lens;
                yield return null;
            }

            // Гарантируем точное конечное значение
            var finalLens = _mainVirtualCamera.Lens; // Перечитываем на случай изменений другими системами (маловероятно, но безопасно)
            finalLens.FieldOfView = targetFOV;
            _mainVirtualCamera.Lens = finalLens;
            _fovTransitionCoroutine = null;
        }

        /// <summary>
        /// Allows setting the velocity provider programmatically after Awake.
        /// </summary>
        public void SetVelocityProvider(IVelocityProvider provider)
        {
            _velocityProvider = provider;
            if (_velocityProvider == null && _enableSpeedEffects)
            {
                UnityEngine.Debug.LogWarning(
                    "CameraEffectsController: Velocity Provider was set to null programmatically. Speed-based FOV effects might be disabled.", this);
            }
        }

        public enum ShakeIntensity
        {
            Light,
            Medium,
            Heavy
        }

        // Опционально: если хочешь отключать эффекты скорости извне
        public void EnableSpeedEffects(bool enable)
        {
            _enableSpeedEffects = enable;
            if (!_enableSpeedEffects && _mainVirtualCamera != null && _fovTransitionCoroutine == null)
            {
                // Плавно вернуть FOV к нормальному, если эффекты выключены и нет явного перехода
                // Можно также сделать это мгновенно или через SetFieldOfView
                StartCoroutine(ResetFOVToNormalCoroutine());
            }
        }

        private IEnumerator ResetFOVToNormalCoroutine()
        {
            if (_mainVirtualCamera == null) yield break;

            var lens = _mainVirtualCamera.Lens;
            float startFOV = lens.FieldOfView;
            float elapsed = 0;
            // Используем _explicitFovChangeSpeed для определения "длительности" возврата
            // Можно сделать это более явным параметром, если нужно
            float estimatedDuration = Mathf.Abs(startFOV - _normalFOV) / (_explicitFovChangeSpeed * 10f); // Примерная длительность
            if (estimatedDuration < 0.1f) estimatedDuration = 0.1f; // Минимальная длительность

            while (elapsed < estimatedDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / estimatedDuration);
                lens.FieldOfView = Mathf.Lerp(startFOV, _normalFOV, t);
                _mainVirtualCamera.Lens = lens;
                yield return null;
            }

            lens.FieldOfView = _normalFOV;
            _mainVirtualCamera.Lens = lens;
        }
    }
}
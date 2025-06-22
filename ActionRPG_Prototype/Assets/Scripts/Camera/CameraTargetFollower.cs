using UnityEngine;

namespace Camera
{
    public class CameraTargetFollower : MonoBehaviour
    {
        [Header("Target Settings")] [Tooltip("The target transform this object will follow.")] [SerializeField]
        private Transform _target;

        [Tooltip("Offset from the target's position.")] [SerializeField]
        private Vector3 _offset = new Vector3(0, 1.5f, 0);

        [Header("Follow Smoothing")]
        [Tooltip("Approximate time it will take to reach the target. A smaller value will reach the target faster.")]
        [SerializeField]
        private float _smoothTime = 0.1f;

        [Tooltip("Optionally, specify a maximum speed for the follow movement.")] [SerializeField]
        private float _maxSpeed = Mathf.Infinity; // По умолчанию нет ограничения

        [Header("Look Ahead Settings")] [SerializeField]
        private bool _enableLookAhead = true;

        [Tooltip("How far ahead to look based on target's velocity.")] [SerializeField]
        private float _lookAheadDistance = 2f;

        [Tooltip("Time it takes for the look-ahead offset to adapt to changes in target velocity. Smaller is faster.")] [SerializeField]
        private float _lookAheadSmoothing = 0.5f;

        private Vector3 _currentVelocitySmoothDamp; // Для SmoothDamp позиции
        private Vector3 _previousTargetPosition;
        private Vector3 _currentLookAheadOffset; // Для плавного изменения look-ahead

        // Флаг для инициализации, чтобы избежать рывка в первом кадре
        private bool _isInitialized = false;

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            // Если объект был выключен и снова включен, и таргет мог измениться,
            // или если Start не был вызван (например, объект был неактивен при старте сцены)
            if (!_isInitialized && _target != null)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (_target == null)
            {
                UnityEngine.Debug.LogWarning("CameraTargetFollower: Target is not set. Follower will not update.", this);
                enabled = false; // Отключаем компонент, если нет цели при инициализации
                return;
            }

            _previousTargetPosition = _target.position;
            _currentLookAheadOffset = Vector3.zero; // Начинаем без смещения на опережение
            // Мгновенно устанавливаем позицию при инициализации, чтобы избежать прыжка
            transform.position = CalculateTargetPositionWithOffset(_target.position);
            _isInitialized = true;
        }

        private void LateUpdate()
        {
            if (!_isInitialized || _target == null) // Если не инициализирован или таргет пропал
            {
                // Можно добавить логику поиска таргета, если он пропал, или просто отключить компонент
                if (_target == null && _isInitialized)
                {
                    UnityEngine.Debug.LogWarning("CameraTargetFollower: Target has been lost/destroyed. Disabling follower.", this);
                    enabled = false;
                }

                return;
            }

            Vector3 currentTargetActualPosition = _target.position;
            Vector3 finalTargetPosition = CalculateTargetPositionWithOffset(currentTargetActualPosition);

            // Look ahead based on movement
            if (_enableLookAhead)
            {
                // Используем Time.smoothDeltaTime для большей стабильности, особенно если Time.timeScale меняется
                float dt = Time.smoothDeltaTime; // Или Time.unscaledDeltaTime, если нужно игнорировать Time.timeScale
                if (dt > Mathf.Epsilon) // Защита от деления на ноль
                {
                    Vector3 targetMovementDirection = (currentTargetActualPosition - _previousTargetPosition) / dt;
                    // Нормализуем, только если есть движение, чтобы избежать NaN от Vector3.zero.normalized
                    if (targetMovementDirection.sqrMagnitude > Mathf.Epsilon)
                    {
                        Vector3 desiredLookAhead = targetMovementDirection.normalized * _lookAheadDistance;
                        // Плавное изменение текущего смещения на опережение
                        if (_lookAheadSmoothing > Mathf.Epsilon)
                        {
                            _currentLookAheadOffset = Vector3.Lerp(
                                _currentLookAheadOffset,
                                desiredLookAhead,
                                dt / _lookAheadSmoothing);
                        }
                        else // Мгновенное применение, если сглаживание 0
                        {
                            _currentLookAheadOffset = desiredLookAhead;
                        }
                    }
                    else // Если нет движения, плавно убираем смещение
                    {
                        if (_lookAheadSmoothing > Mathf.Epsilon)
                        {
                            _currentLookAheadOffset = Vector3.Lerp(_currentLookAheadOffset, Vector3.zero, dt / _lookAheadSmoothing);
                        }
                        else
                        {
                            _currentLookAheadOffset = Vector3.zero;
                        }
                    }
                }

                finalTargetPosition += _currentLookAheadOffset;
            }
            else
            {
                // Если look ahead выключен, плавно убираем существующее смещение
                float dt = Time.smoothDeltaTime;
                if (_lookAheadSmoothing > Mathf.Epsilon && dt > Mathf.Epsilon)
                {
                    _currentLookAheadOffset = Vector3.Lerp(_currentLookAheadOffset, Vector3.zero, dt / _lookAheadSmoothing);
                    finalTargetPosition += _currentLookAheadOffset; // Все еще применяем угасающее смещение
                }
                else
                {
                    _currentLookAheadOffset = Vector3.zero;
                }
            }

            // Smooth follow
            if (_smoothTime > Mathf.Epsilon)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    finalTargetPosition,
                    ref _currentVelocitySmoothDamp,
                    _smoothTime,
                    _maxSpeed, // Добавлено ограничение максимальной скорости
                    Time.deltaTime // SmoothDamp использует Time.deltaTime для корректной работы с физикой и анимацией
                );
            }
            else // Мгновенное следование, если smoothTime равен 0
            {
                transform.position = finalTargetPosition;
            }

            _previousTargetPosition = currentTargetActualPosition;
        }

        private Vector3 CalculateTargetPositionWithOffset(Vector3 targetPos)
        {
            return targetPos + _offset;
        }

        /// <summary>
        /// Sets a new target for the follower.
        /// </summary>
        /// <param name="newTarget">The new transform to follow. Can be null to stop following.</param>
        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
            if (_target != null)
            {
                // Если компонент был выключен из-за отсутствия цели, включаем его
                if (!enabled) enabled = true;
                // Переинициализируем, чтобы правильно установить начальные значения
                // и избежать рывка, если новая цель далеко от старой.
                Initialize();
            }
            else
            {
                UnityEngine.Debug.LogWarning("CameraTargetFollower: Target set to null. Follower will be disabled.", this);
                _isInitialized = false; // Сбрасываем флаг, так как цели больше нет
                enabled = false; // Отключаем компонент, если цель null
            }
        }

        /// <summary>
        /// Instantly teleports the follower to the target's position (with offset and look-ahead if enabled).
        /// Useful after loading a new scene or teleporting the player.
        /// </summary>
        public void SnapToTarget()
        {
            if (_target == null) return;

            Vector3 currentTargetActualPosition = _target.position;
            Vector3 finalTargetPosition = CalculateTargetPositionWithOffset(currentTargetActualPosition);
            if (_enableLookAhead)
            {
                // При снапе можно либо не учитывать look-ahead, либо рассчитать его на основе текущей скорости,
                // но это может быть не так важно, как простое позиционирование.
                // Для простоты, при снапе можно временно игнорировать _currentLookAheadOffset или сбросить его.
                // Здесь мы его учтем, если он уже был рассчитан.
                finalTargetPosition += _currentLookAheadOffset;
            }

            transform.position = finalTargetPosition;
            _previousTargetPosition = currentTargetActualPosition; // Обновляем для следующего кадра
            _currentVelocitySmoothDamp = Vector3.zero; // Сбрасываем скорость SmoothDamp
        }

        // Для отладки в редакторе
        private void OnDrawGizmosSelected()
        {
            if (_target != null)
            {
                Gizmos.color = Color.yellow;
                Vector3 targetPosWithOffset = _target.position + _offset;
                Gizmos.DrawLine(transform.position, targetPosWithOffset);
                Gizmos.DrawWireSphere(targetPosWithOffset, 0.2f);

                if (_enableLookAhead && _isInitialized) // _isInitialized чтобы не рисовать до старта
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(targetPosWithOffset, targetPosWithOffset + _currentLookAheadOffset);
                    Gizmos.DrawWireSphere(targetPosWithOffset + _currentLookAheadOffset, 0.15f);
                }
            }
        }
    }
}
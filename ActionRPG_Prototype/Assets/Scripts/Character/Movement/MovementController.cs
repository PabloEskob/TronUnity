// MovementController.cs

using System;
using Config.Movement;
using Core.Events;
using Core.Events.Messages;
using Core.Input.Interfaces;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using UnityVectorExtensions = Character.Movement.States.Base.UnityVectorExtensions;

namespace Character.Movement
{
    [AddComponentMenu("Character/Movement Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementPhysics))]
    public sealed class MovementController : MonoBehaviour
    {
        #region Constants

        static class Constants
        {
            public const float MinMovementThreshold = 0.01f;
            public const float MinInputMagnitude = 0.1f;
            public const float DeadZone = 0.001f;
        }

        #endregion

        #region Enums

        public enum ForwardMode
        {
            Camera,
            Player,
            World
        }

        public enum UpMode
        {
            Player,
            World
        }

        #endregion

        #region Serialized Fields

        [Header("Configuration")] [SerializeField]
        MovementConfig _config;

        [Header("Movement Settings")] [SerializeField]
        float _damping = 0.5f;

        [SerializeField] bool _strafeMode = false;
        [SerializeField] ForwardMode _inputForward = ForwardMode.Camera;
        [SerializeField] UpMode _upMode = UpMode.World;

        [Header("Rotation Settings")] [SerializeField]
        float _rotationDamping = 0.1f;

        [SerializeField] float _strafeRotationMultiplier = 0.5f;

        [Header("Components")] [SerializeField]
        CharacterController _characterController;

        [SerializeField] MovementPhysics _physics;
        [SerializeField] Camera _cameraOverride;

        #endregion

        #region Private Fields

        IMovementInput _input;
        IEventBus _bus;

        Vector3 _currentVelocity;
        Vector3 _lastInput;
        Vector3 _lastRawInput;
        float _currentSpeed;
        bool _isSprinting;

        // Gimbal lock prevention
        bool _inTopHemisphere = true;
        float _timeInHemisphere = 100f;
        readonly Quaternion _upsideDown = Quaternion.AngleAxis(180, Vector3.left);

        #endregion

        #region Properties

        public MovementConfig Config => _config;
        public CharacterController CharacterController => _characterController;
        public MovementPhysics Physics => _physics;
        public IMovementInput Input => _input;

        public Vector3 CurrentVelocity => _currentVelocity;
        public float CurrentSpeed => _currentSpeed;
        public bool IsMoving => _lastInput.sqrMagnitude > Constants.MinMovementThreshold;
        public bool IsGrounded => _physics.IsGrounded;
        public bool IsSprinting => _isSprinting;
        public bool IsStrafing => _strafeMode;

        public Camera ActiveCamera => _cameraOverride != null ? _cameraOverride : Camera.main;
        Vector3 UpDirection => _upMode == UpMode.World ? Vector3.up : transform.up;

        #endregion

        #region Dependency Injection

        [Inject]
        public void Construct(IMovementInput input, IEventBus bus)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            ValidateComponents();
        }

        void OnEnable()
        {
            _currentVelocity = Vector3.zero;
            _isSprinting = false;
        }

        void OnValidate()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();
            if (_physics == null)
                _physics = GetComponent<MovementPhysics>();
        }

        #endregion

        #region Public Methods

        public void Move(Vector3 direction, float speed)
        {
            if (!CanMove()) return;

            if (direction.sqrMagnitude < Constants.MinMovementThreshold)
            {
                ApplyDeceleration();
                return;
            }

            // Нормализуем и сохраняем направление
            direction = direction.normalized;
            _lastInput = direction;
            _currentSpeed = speed;

            // Применяем демпфирование для плавного движения
            var targetVelocity = direction * speed;
            ApplyAcceleration(targetVelocity);

            // Движение персонажа
            var movement = _currentVelocity * Time.deltaTime;
            _characterController.Move(movement);

            // Публикуем событие
            _bus.Publish(new PlayerMoved(direction));
        }

        public void Rotate(Vector3 direction, float rotationSpeed)
        {
            if (direction.sqrMagnitude < Constants.DeadZone) return;

            // В режиме стрейфа поворачиваемся медленнее
            if (_strafeMode)
            {
                RotateStrafe(direction, rotationSpeed);
            }
            else
            {
                RotateFree(direction, rotationSpeed);
            }
        }

        public void Stop()
        {
            _lastInput = Vector3.zero;
            _currentSpeed = 0f;
            ApplyDeceleration();
            _bus.Publish(new PlayerStopped());
        }

        public Vector3 GetMovementDirection()
        {
            if (_input == null) return Vector3.zero;

            var rawInput = new Vector3(_input.MovementVector.x, 0, _input.MovementVector.y);
            if (rawInput.magnitude < Constants.MinInputMagnitude)
                return Vector3.zero;

            // Получаем систему координат для ввода
            var inputFrame = GetInputFrame(Vector3.Dot(rawInput, _lastRawInput) < 0.8f);
            _lastRawInput = rawInput;

            // Преобразуем ввод в мировые координаты
            var worldInput = inputFrame * rawInput;
            if (worldInput.sqrMagnitude > 1)
                worldInput.Normalize();

            return worldInput;
        }

        public void SetStrafeMode(bool enabled)
        {
            _strafeMode = enabled;
        }

        public void SetSprinting(bool sprinting)
        {
            _isSprinting = sprinting;
        }

        #endregion

        #region Private Methods

        void ValidateComponents()
        {
            if (_characterController == null)
            {
                Debug.LogError($"[MovementController] CharacterController is missing on {name}!", this);
                enabled = false;
            }

            if (_physics == null)
            {
                Debug.LogError($"[MovementController] MovementPhysics is missing on {name}!", this);
                enabled = false;
            }

            if (_config == null)
            {
                Debug.LogError($"[MovementController] MovementConfig is not assigned on {name}!", this);
            }
        }

        bool CanMove()
        {
            return enabled &&
                   _characterController != null &&
                   _characterController.enabled &&
                   _config != null;
        }

        void ApplyAcceleration(Vector3 targetVelocity)
        {
            // Используем SLERP для плавного изменения направления
            if (Vector3.Angle(_currentVelocity, targetVelocity) < 100)
            {
                _currentVelocity = Vector3.Slerp(
                    _currentVelocity,
                    targetVelocity,
                    Damper.Damp(1, _damping, Time.deltaTime));
            }
            else
            {
                // Для резких поворотов используем линейную интерполяцию
                _currentVelocity += Damper.Damp(
                    targetVelocity - _currentVelocity,
                    _damping,
                    Time.deltaTime);
            }
        }

        void ApplyDeceleration()
        {
            _currentVelocity = Vector3.Lerp(
                _currentVelocity,
                Vector3.zero,
                Damper.Damp(1, _damping * 0.5f, Time.deltaTime));
        }

        void RotateFree(Vector3 direction, float rotationSpeed)
        {
            // Не поворачиваемся, если движемся назад в режиме Player
            var forward = transform.forward;
            if (_inputForward == ForwardMode.Player && Vector3.Dot(forward, direction) < 0)
                direction = -direction;

            var targetRotation = Quaternion.LookRotation(direction, UpDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Damper.Damp(1, rotationSpeed, Time.deltaTime));
        }

        void RotateStrafe(Vector3 direction, float rotationSpeed)
        {
            // В режиме стрейфа персонаж всегда смотрит вперед относительно камеры
            var cameraForward = ActiveCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            // Добавляем небольшой поворот в сторону движения
            var angle = Vector3.SignedAngle(cameraForward, direction, Vector3.up);
            angle = Mathf.Clamp(angle * _strafeRotationMultiplier, -45f, 45f);

            var targetRotation = Quaternion.LookRotation(cameraForward, UpDirection) *
                                 Quaternion.Euler(0, angle, 0);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Damper.Damp(1, rotationSpeed * 2f, Time.deltaTime));
        }

        Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            var frame = Quaternion.identity;

            switch (_inputForward)
            {
                case ForwardMode.Camera:
                    frame = ActiveCamera.transform.rotation;
                    break;
                case ForwardMode.Player:
                    return transform.rotation;
                case ForwardMode.World:
                    break;
            }

            // Простая проекция на плоскость игрока для большинства случаев
            var playerUp = transform.up;
            var frameUp = frame * Vector3.up;

            // Проверяем, не перевернут ли игрок относительно системы ввода
            const float BlendTime = 2f;
            _timeInHemisphere += Time.deltaTime;
            bool inTopHemisphere = Vector3.Dot(frameUp, playerUp) >= 0;

            if (inTopHemisphere != _inTopHemisphere)
            {
                _inTopHemisphere = inTopHemisphere;
                _timeInHemisphere = Mathf.Max(0, BlendTime - _timeInHemisphere);
            }

            // Простой случай - игрок не наклонен
            var axis = Vector3.Cross(frameUp, playerUp);
            if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
                return frame;

            // Наклоняем систему координат под игрока
            var angle = UnityVectorExtensions.SignedAngle(frameUp, playerUp, axis);
            var frameA = Quaternion.AngleAxis(angle, axis) * frame;

            // Обработка gimbal lock при переворачивании
            Quaternion frameB = frameA;
            if (!inTopHemisphere || _timeInHemisphere < BlendTime)
            {
                frameB = frame * _upsideDown;
                var axisB = Vector3.Cross(frameB * Vector3.up, playerUp);
                if (axisB.sqrMagnitude > 0.001f)
                    frameB = Quaternion.AngleAxis(180f - angle, axisB) * frameB;
            }

            if (inputDirectionChanged)
                _timeInHemisphere = BlendTime;

            if (_timeInHemisphere >= BlendTime)
                return inTopHemisphere ? frameA : frameB;

            return inTopHemisphere
                ? Quaternion.Slerp(frameB, frameA, _timeInHemisphere / BlendTime)
                : Quaternion.Slerp(frameA, frameB, _timeInHemisphere / BlendTime);
        }

        #endregion

        #region Debug

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Направление движения
            if (IsMoving)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, _currentVelocity.normalized * 2f);
            }

            // Направление взгляда
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * 1.5f);
        }
#endif

        #endregion
    }

    // Вспомогательный класс для демпфирования
    public static class Damper
    {
        public static float Damp(float current, float damping, float deltaTime)
        {
            return 1f - Mathf.Exp(-damping * deltaTime);
        }

        public static Vector3 Damp(Vector3 current, float damping, float deltaTime)
        {
            return current * (1f - Mathf.Exp(-damping * deltaTime));
        }
    }
}
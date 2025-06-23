using Core.Input.Interfaces;
using UnityEngine;
using Config.Movement;
using Core.Events;
using VContainer;
using System;

namespace Character.Movement
{
    [AddComponentMenu("Character/Movement Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementStateMachine))]
    [RequireComponent(typeof(MovementPhysics))]
    public sealed class MovementController : MonoBehaviour
    {
        #region Constants

        private static class Constants
        {
            public const float MinMovementThreshold = 0.01f;
            public const float MinInputMagnitude = 0.1f;
            public const float DeadZone = 0.001f;
        }

        #endregion

        #region Serialized Fields

        [Header("Configuration")] [SerializeField]
        private MovementConfig _config;

        [Header("Components")] [SerializeField]
        private CharacterController _characterController;

        [SerializeField] private MovementPhysics _physics;

        #endregion

        #region Private Fields

        private IMovementInput _input;
        private Camera _mainCamera;
        private Transform _cameraTransform;
        private Vector3 _lastMovementDirection;
        private Vector3 _smoothedMovementDirection;
        private float _currentSpeed;

        #endregion

        #region Properties

        public MovementConfig Config => _config;
        public CharacterController CharacterController => _characterController;
        public MovementPhysics Physics => _physics;
        public IMovementInput Input => _input;

        public Vector3 LastMovementDirection => _lastMovementDirection;
        public Vector3 CurrentVelocity => _characterController.velocity;
        public float CurrentSpeed => _currentSpeed;
        public bool IsMoving => _lastMovementDirection.sqrMagnitude > Constants.MinMovementThreshold;
        public bool IsGrounded => _physics.IsGrounded;

        #endregion

        #region Dependency Injection

        [Inject]
        public void Construct(IMovementInput input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            ValidateComponents();
            CacheComponents();
        }

        private void Start()
        {
            InitializeCamera();
            ValidateDependencies();
        }

        private void OnValidate()
        {
            // Автоматическое заполнение компонентов в редакторе
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
                StopMovement();
                return;
            }

            // Нормализуем направление
            var normalizedDirection = direction.normalized;
            _lastMovementDirection = normalizedDirection;
            _currentSpeed = speed;

            // Применяем движение
            var movement = normalizedDirection * speed * Time.deltaTime;
            _characterController.Move(movement);

            // Вызываем событие
            GameEvents.InvokePlayerMoved(normalizedDirection);
        }

        public void Rotate(Vector3 direction, float rotationSpeed)
        {
            if (direction == Vector3.zero || direction.sqrMagnitude < Constants.DeadZone)
                return;

            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        public void Stop()
        {
            StopMovement();
        }

        public Vector3 GetMovementDirection()
        {
            if (_input == null) return Vector3.zero;

            var inputVector = _input.MovementVector;
            if (inputVector.magnitude < Constants.MinInputMagnitude)
                return Vector3.zero;

            return CalculateCameraRelativeDirection(inputVector);
        }

        public void SetMovementEnabled(bool enabled)
        {
            _characterController.enabled = enabled;
        }

        public void Teleport(Vector3 position)
        {
            _characterController.enabled = false;
            transform.position = position;
            _characterController.enabled = true;
        }

        #endregion

        #region Private Methods

        private void ValidateComponents()
        {
            if (_characterController == null)
            {
                _characterController = GetComponent<CharacterController>();
                if (_characterController == null)
                {
                    Debug.LogError($"[MovementController] CharacterController is missing on {name}!", this);
                    enabled = false;
                }
            }

            if (_physics == null)
            {
                _physics = GetComponent<MovementPhysics>();
                if (_physics == null)
                {
                    Debug.LogError($"[MovementController] MovementPhysics is missing on {name}!", this);
                    enabled = false;
                }
            }

            if (_config == null)
            {
                Debug.LogError($"[MovementController] MovementConfig is not assigned on {name}!", this);
            }
        }

        private void CacheComponents()
        {
            // Кешируем компоненты для производительности
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            if (_physics == null)
                _physics = GetComponent<MovementPhysics>();
        }

        private void InitializeCamera()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                _cameraTransform = _mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("[MovementController] Main camera not found!");
            }
        }

        private void ValidateDependencies()
        {
            if (_input == null)
            {
                Debug.LogError($"[MovementController] Input not injected on {name}! " +
                               "Make sure VContainer is properly configured.", this);
                enabled = false;
            }
        }

        private bool CanMove()
        {
            return enabled &&
                   _characterController != null &&
                   _characterController.enabled &&
                   _config != null;
        }

        private void StopMovement()
        {
            _lastMovementDirection = Vector3.zero;
            _currentSpeed = 0f;
            GameEvents.InvokePlayerStopped();
        }

        private Vector3 CalculateCameraRelativeDirection(Vector2 input)
        {
            if (_cameraTransform == null)
                return new Vector3(input.x, 0, input.y);

            // Получаем направления камеры
            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;

            // Проецируем на горизонтальную плоскость
            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            // Вычисляем направление относительно камеры
            var desiredDirection = forward * input.y + right * input.x;
            return desiredDirection.normalized;
        }

        #endregion

        #region Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Отрисовка направления движения
            if (IsMoving)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, _lastMovementDirection * 2f);
            }

            // Отрисовка скорости
            if (_characterController != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + Vector3.up * 0.1f,
                    _characterController.velocity.normalized);
            }
        }
#endif

        #endregion
    }
}
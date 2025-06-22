using Core.Events;
using Core.Input.Interfaces;
using Core.Services;
using UnityEngine;
using Config.Movement;
using Core.Input;

namespace Character.Movement
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementStateMachine))]
    public class MovementController : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private MovementConfig _config;

        [Header("Components")] [SerializeField]
        private CharacterController _characterController;

        [SerializeField] private MovementPhysics _physics;

        private IMovementInput _input;
        private Vector3 _lastMovementDirection;

        public MovementConfig Config => _config;
        public CharacterController CharacterController => _characterController;
        public MovementPhysics Physics => _physics;
        public IMovementInput Input => _input;

        public Vector3 LastMovementDirection => _lastMovementDirection;
        public bool IsMoving => _lastMovementDirection.sqrMagnitude > 0.01f;

        private void Awake()
        {
            ValidateComponents();
        }

        private void Start()
        {
            var inputManager = ServiceLocator.Instance.GetService<InputManager>();
            _input = inputManager.Movement;
        }

        private void ValidateComponents()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            if (_physics == null)
                _physics = GetComponent<MovementPhysics>();

            if (_config == null)
                UnityEngine.Debug.LogError($"MovementConfig is missing on {gameObject.name}");
        }

        public void Move(Vector3 direction, float speed)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                _lastMovementDirection = direction.normalized;
                var motion = _lastMovementDirection * speed * Time.deltaTime;
                _characterController.Move(motion);

                GameEvents.OnPlayerMoved.Invoke(_lastMovementDirection);
            }
        }

        public void Rotate(Vector3 direction, float rotationSpeed)
        {
            if (direction == Vector3.zero) return;

            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        public void Stop()
        {
            _lastMovementDirection = Vector3.zero;
            GameEvents.OnPlayerStopped.Invoke();
        }

        public Vector3 GetMovementDirection()
        {
            var input = _input.MovementVector;
            if (input.magnitude < 0.1f) return Vector3.zero;

            var cameraTransform = UnityEngine.Camera.main.transform;
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }
    }
}
using Core.Events;
using Core.Input.Interfaces;
using Core.Services;
using UnityEngine;
using Config.Movement;
using Core.Input;

namespace Character.Movement
{
    [AddComponentMenu("Character/Movement Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementStateMachine))]
    public sealed class MovementController : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private MovementConfig _config;

        [Header("Components")] [SerializeField]
        private CharacterController _characterController;

        [SerializeField] private MovementPhysics _physics;

        private IMovementInput _input;
        private Vector3 _lastDir;
        private UnityEngine.Camera _cam;

        #region Properties

        public MovementConfig Config => _config;
        public CharacterController CharacterController => _characterController;
        public MovementPhysics Physics => _physics;
        public IMovementInput Input => _input;
        public Vector3 LastMovementDirection => _lastDir;
        public bool IsMoving => _lastDir.sqrMagnitude > 0.01f;

        #endregion

        private void Awake()
        {
            if (_characterController == null) _characterController = GetComponent<CharacterController>();
            if (_physics == null) _physics = GetComponent<MovementPhysics>();
            if (_config == null) Debug.LogError($"{nameof(MovementConfig)} missing on {name}", this);
        }

        private void Start()
        {
            _input = ServiceLocator.Get<InputManager>().Movement;
            _cam = UnityEngine.Camera.main;
        }

        public void Move(Vector3 dir, float speed)
        {
            if (dir.sqrMagnitude < 0.01f) return;

            _lastDir = dir.normalized;
            _characterController.Move(_lastDir * speed * Time.deltaTime);
            GameEvents.InvokePlayerMoved(_lastDir);
        }

        public void Rotate(Vector3 dir, float rotSpeed)
        {
            if (dir == Vector3.zero) return;
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotSpeed * Time.deltaTime);
        }

        public void Stop()
        {
            _lastDir = Vector3.zero;
            GameEvents.InvokePlayerStopped();
        }

        public Vector3 GetMovementDirection()
        {
            Vector2 raw = _input.MovementVector;
            if (raw.magnitude < 0.1f) return Vector3.zero;

            // Project camera‑space input onto world‑space XZ plane
            Vector3 f = _cam.transform.forward;
            f.y = 0;
            f.Normalize();
            Vector3 r = _cam.transform.right;
            r.y = 0;
            r.Normalize();
            return (f * raw.y + r * raw.x).normalized;
        }
    }
}
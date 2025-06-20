// CharacterMovementController.cs

using System.Collections.Generic;
using Assets.Scripts.Movement.Interface;
using Config.Movement;
using Movement.State;
using UnityEngine;
using RunState = Movement.State.RunState;

namespace CharacterMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovementController : MonoBehaviour, IMovementHandler
    {
        [SerializeField] private MovementConfig _config;

        private CharacterController _characterController;
        private IMovementInput _input;
        private IMovementState _currentState;
        private Dictionary<System.Type, IMovementState> _states;

        private Vector3 _velocity;
        private bool _isGrounded = true;
        private float _lastDodgeTime;

        public MovementConfig Config => _config;
        public bool IsGrounded => _isGrounded;
        public bool CanDodge => Time.time - _lastDodgeTime > _config.dodgeCooldown;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _input = GetComponent<IMovementInput>();
            InitializeStates();
        }

        private void InitializeStates()
        {
            _states = new Dictionary<System.Type, IMovementState>
            {
                { typeof(IdleState), new IdleState(this, _input) },
                { typeof(WalkState), new WalkState(this, _input, _config) },
                { typeof(RunState), new RunState(this, _input, _config) },
                { typeof(DodgeState), new DodgeState(this, _input, _config) }
            };

            ChangeState<IdleState>();
        }

        private void Update()
        {
            CheckGrounded();
            ApplyGravity();
            _currentState?.Execute();
        }

        public void ChangeState<T>() where T : IMovementState
        {
            var type = typeof(T);
            if (_states.ContainsKey(type))
            {
                _currentState?.Exit();
                _currentState = _states[type];
                _currentState.Enter();
            }
        }

        public void Move(Vector3 direction, float speed)
        {
            var motion = direction * speed;
            _characterController.Move(motion * Time.deltaTime);
        }

        public void Rotate(Vector3 direction, float rotationSpeed)
        {
            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        public void Stop()
        {
            _velocity.x = 0;
            _velocity.z = 0;
        }

        private void CheckGrounded()
        {
            _isGrounded = Physics.CheckSphere(
                transform.position - Vector3.up * 0.1f,
                _config.groundCheckDistance,
                _config.groundLayer
            );
        }

        private void ApplyGravity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            else
            {
                _velocity.y += _config.gravity * Time.deltaTime;
            }

            _characterController.Move(_velocity * Time.deltaTime);
        }

        public void SetDodgeTime()
        {
            _lastDodgeTime = Time.time;
        }
    }
}
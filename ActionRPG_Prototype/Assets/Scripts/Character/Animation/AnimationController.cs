using Character.Movement;
using Character.Movement.States;
using UnityEngine;

namespace Character.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private Animator _animator;
        private MovementController _movement;
        private MovementStateMachine _stateMachine;

        // Animation parameter hashes
        private readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int _isRunningHash = Animator.StringToHash("IsRunning");
        private readonly int _verticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        private readonly int _dodgeHash = Animator.StringToHash("Dodge");
        private readonly int _jumpHash = Animator.StringToHash("Jump");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponentInParent<MovementController>();
            _stateMachine = GetComponentInParent<MovementStateMachine>();
        }

        private void Update()
        {
            UpdateMovementAnimations();
            UpdateStateAnimations();
        }

        private void UpdateMovementAnimations()
        {
            // Calculate movement speed
            var velocity = _movement.CharacterController.velocity;
            var horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;

            // Update animator parameters
            _animator.SetFloat(_speedHash, horizontalSpeed);
            _animator.SetBool(_isGroundedHash, _movement.Physics.IsGrounded);
            _animator.SetFloat(_verticalVelocityHash, velocity.y);
            _animator.SetBool(_isRunningHash, _stateMachine.IsInState<RunState>());
        }

        private void UpdateStateAnimations()
        {
            // Trigger state-specific animations
            if (_stateMachine.IsInState<DodgeState>())
            {
                _animator.SetTrigger(_dodgeHash);
            }

            if (_stateMachine.IsInState<JumpState>())
            {
                _animator.SetTrigger(_jumpHash);
            }
        }

        public void PlayAttackAnimation(int attackIndex)
        {
            _animator.SetTrigger($"Attack{attackIndex}");
        }

        public void SetCombatStance(bool inCombat)
        {
            _animator.SetBool("InCombat", inCombat);
        }

        public void SetAnimationSpeed(float speed)
        {
            _animator.speed = speed;
        }
    }
}
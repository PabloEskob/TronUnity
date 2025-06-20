// CharacterStateManager.cs

using CharacterMovement;
using UnityEngine;

namespace Manager
{
    public class CharacterStateManager : MonoBehaviour
    {
        private Animator _animator;
        private CharacterMovementController _movementController;

        private readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int _dodgeTriggerHash = Animator.StringToHash("Dodge");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movementController = GetComponent<CharacterMovementController>();
        }

        private void Update()
        {
            UpdateAnimatorParameters();
        }

        private void UpdateAnimatorParameters()
        {
            var velocity = _movementController.GetComponent<CharacterController>().velocity;
            var horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;

           // _animator.SetFloat(_speedHash, horizontalSpeed);
           // _animator.SetBool(_isGroundedHash, _movementController.IsGrounded);
        }

        public void TriggerDodgeAnimation()
        {
            _animator.SetTrigger(_dodgeTriggerHash);
        }
    }
}
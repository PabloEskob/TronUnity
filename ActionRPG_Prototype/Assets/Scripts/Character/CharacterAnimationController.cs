// CharacterAnimationController.cs

using System;
using Character.Movement;
using UnityEngine;

namespace Character
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Animator _animator;

        [SerializeField] private MovementController _movementController;
        [SerializeField] private MovementPhysics _movementPhysics;

        [Header("Animation Parameters")] [SerializeField]
        private string _speedParam = "Speed";

        [SerializeField] private string _isGroundedParam = "IsGrounded";
        [SerializeField] private string _verticalVelocityParam = "VerticalVelocity";
        [SerializeField] private string _dodgeTrigger = "Dodge";
        [SerializeField] private string _attackTrigger = "Attack";

        // Hash для оптимизации
        private int _speedHash;
        private int _isGroundedHash;
        private int _verticalVelocityHash;
        private int _dodgeHash;
        private int _attackHash;
        private CharacterController _characterController;

        // События для эффектов
        public event Action<int> OnFootstep;
        public event Action OnDodgeStart;
        public event Action OnAttackHit;

        private void Start()
        {
            _characterController = _movementController.GetComponent<CharacterController>();
        }

        private void Awake()
        {
            CacheAnimationHashes();
        }

        private void CacheAnimationHashes()
        {
            _speedHash = Animator.StringToHash(_speedParam);
            _isGroundedHash = Animator.StringToHash(_isGroundedParam);
            _verticalVelocityHash = Animator.StringToHash(_verticalVelocityParam);
            _dodgeHash = Animator.StringToHash(_dodgeTrigger);
            _attackHash = Animator.StringToHash(_attackTrigger);
        }

        private void Update()
        {
            if (!_animator || !_movementController) return;

            UpdateMovementAnimations();
        }

        private void UpdateMovementAnimations()
        {
            var velocity = _characterController.velocity;
            float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;

            _animator.SetFloat(_speedHash, horizontalSpeed);
            _animator.SetBool(_isGroundedHash, _movementPhysics.IsGrounded);
            _animator.SetFloat(_verticalVelocityHash, velocity.y);
        }

        public void TriggerDodge()
        {
            _animator.SetTrigger(_dodgeHash);
            OnDodgeStart?.Invoke();
        }

        public void TriggerAttack(int attackIndex)
        {
            _animator.SetTrigger(_attackTrigger);
            _animator.SetInteger("AttackIndex", attackIndex);
        }

        // Animation Events (вызываются из анимаций)
        public void AnimEvent_Footstep(int foot)
        {
            OnFootstep?.Invoke(foot);
        }

        public void AnimEvent_AttackHit()
        {
            OnAttackHit?.Invoke();
        }
    }
}
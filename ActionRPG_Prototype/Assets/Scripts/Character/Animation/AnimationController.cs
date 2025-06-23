using Character.Movement;
using Character.Movement.States;
using UnityEngine;

namespace Character.Animation
{
    [AddComponentMenu("Character/Animation Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationController : MonoBehaviour
    {
        private Animator _anim;
        private MovementController _move;
        private MovementStateMachine _state;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int VerticalVelHash = Animator.StringToHash("VerticalVelocity");
        private static readonly int DodgeTriggerHash = Animator.StringToHash("Dodge");
        private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");

        private bool _wasDodging;
        private bool _wasJumping;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _move = GetComponentInParent<MovementController>(true);
            _state = GetComponentInParent<MovementStateMachine>(true);

            if (_anim == null || _move == null || _state == null)
            {
                Debug.LogError("[AnimationController] Missing dependencies", this);
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            if (!enabled) return;

            Vector3 v = _move.CharacterController.velocity;
            float hSpeed = new Vector3(v.x, 0, v.z).magnitude;

            _anim.SetFloat(SpeedHash, hSpeed);
            _anim.SetBool(IsGroundedHash, _move.Physics.IsGrounded);
            _anim.SetFloat(VerticalVelHash, v.y);
            _anim.SetBool(IsRunningHash, _state.IsInState<RunState>());

            bool isDodging = _state.IsInState<DodgeState>();
            if (isDodging && !_wasDodging) _anim.SetTrigger(DodgeTriggerHash);
            _wasDodging = isDodging;

            bool isJumping = _state.IsInState<JumpState>();
            if (isJumping && !_wasJumping) _anim.SetTrigger(JumpTriggerHash);
            _wasJumping = isJumping;
        }

        #region Combat helpers

        public void PlayAttackAnimation(int index) => _anim.SetTrigger($"Attack{index}");
        public void SetCombatStance(bool inCombat) => _anim.SetBool("InCombat", inCombat);
        public void SetAnimationSpeed(float speed) => _anim.speed = speed;

        #endregion
    }
}
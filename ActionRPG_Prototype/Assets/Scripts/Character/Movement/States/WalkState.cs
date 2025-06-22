using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public sealed class WalkState : MovementStateBase
    {
        private float _lastDodge;
        private const float DodgeCooldown = 0.5f;

        public WalkState(MovementStateMachine m) : base(m) {}

        public override void Execute()
        {
            if (!IsGrounded) { _machine.ChangeState<FallState>(); return; }

            if (InputDir.sqrMagnitude < 0.1f) { _machine.ChangeState<IdleState>(); return; }
            if (_controller.Input.IsRunning && HasStamina()) { _machine.ChangeState<RunState>(); return; }
            if (_controller.Input.IsDodging && CanDodge())  { _machine.ChangeState<DodgeState>(); return; }
            if (_controller.Input.IsJumping)                { _machine.ChangeState<JumpState>(); return; }
        }

        public override void FixedExecute()
        {
            _controller.Move(InputDir, _controller.Config.walkSpeed);
            _controller.Rotate(InputDir, _controller.Config.walkRotationSpeed);
        }

        public override void Exit()
        {
            if (_machine.CurrentStateType == typeof(DodgeState))
                _lastDodge = Time.time;
        }

        private bool HasStamina() => _controller.GetComponent<Character.Stats.CharacterStats>().CurrentStamina > 0;
        private bool CanDodge()   => Time.time - _lastDodge > DodgeCooldown;
    }
}

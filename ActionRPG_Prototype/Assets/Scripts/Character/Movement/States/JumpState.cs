using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public sealed class JumpState : MovementStateBase
    {
        private float _jumpStart;
        private bool  _impulseApplied;

        public JumpState(MovementStateMachine m) : base(m) {}

        public override void Enter()
        {
            _jumpStart      = Time.time;
            _impulseApplied = false;
        }

        public override void Execute()
        {
            // Air control
            if (InputDir.sqrMagnitude > 0.1f)
            {
                _controller.Move(InputDir, _controller.Config.walkSpeed * 0.7f);
                _controller.Rotate(InputDir, _controller.Config.walkRotationSpeed);
            }

            // Falling
            if (_controller.Physics.Velocity.y < 0)
            {
                _machine.ChangeState<FallState>();
            }
        }

        public override void FixedExecute()
        {
            if (_impulseApplied) return;

            _controller.Physics.Jump(_controller.Config.jumpForce);
            _impulseApplied = true;
        }

        public override bool CanTransitionTo<T>()
        {
            // No dodge within first 0.2s of jump
            if (typeof(T) == typeof(DodgeState))
                return Time.time - _jumpStart > 0.2f;
            return true;
        }
    }
}
using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public sealed class JumpState : MovementStateBase
    {
        float _jumpStart;
        bool  _impulseApplied;

        public JumpState(MovementStateMachine m) : base(m) { }

        public override void Enter()
        {
            _jumpStart      = Time.time;
            _impulseApplied = false;
        }

        public override void Execute()
        {
            if (InputDir.sqrMagnitude > .1f)    // air-control
            {
                _controller.Move  (InputDir, _controller.Config.walkSpeed * .7f);
                _controller.Rotate(InputDir, _controller.Config.walkRotationSpeed);
            }

            if (_controller.Physics.Velocity.y < 0)   // apex reached
                _machine.ChangeState<FallState>();
        }

        public override void FixedExecute()
        {
            if (_impulseApplied) return;
            _controller.Physics.Jump(_controller.Config.jumpForce);
            _impulseApplied = true;
        }

        public override bool CanTransitionTo<T>()
            => typeof(T) != typeof(DodgeState) || Time.time - _jumpStart > .2f;
    }
}
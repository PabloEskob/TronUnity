using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public sealed class FallState : MovementStateBase
    {
        public FallState(MovementStateMachine m) : base(m) { }

        public override void Execute()
        {
            if (InputDir.sqrMagnitude > .1f)      // air-control
            {
                _controller.Move  (InputDir, _controller.Config.walkSpeed * .5f);
                _controller.Rotate(InputDir, _controller.Config.walkRotationSpeed * .5f);
            }

            if (IsGrounded)
                _machine.ChangeState(InputDir.sqrMagnitude > .1f
                    ? typeof(WalkState) : typeof(IdleState));
        }
    }
}
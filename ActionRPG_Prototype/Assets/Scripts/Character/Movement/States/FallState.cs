using Character.Movement.States.Base;

namespace Character.Movement.States
{
    public sealed class FallState : MovementStateBase
    {
        public FallState(MovementStateMachine m) : base(m) {}

        public override void Execute()
        {
            if (InputDir.sqrMagnitude > .1f)
            {
                _controller.Move(InputDir, _controller.Config.walkSpeed * 0.5f);
                _controller.Rotate(InputDir, _controller.Config.walkRotationSpeed * 0.5f);
            }
            if (IsGrounded)
                _machine.ChangeState(InputDir.sqrMagnitude > .1f ? typeof(WalkState) : typeof(IdleState));
        }
    }
}
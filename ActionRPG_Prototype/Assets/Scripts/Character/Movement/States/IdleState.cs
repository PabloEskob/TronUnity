using Character.Movement.States.Base;
using Core.Events;

namespace Character.Movement.States
{
    public sealed class IdleState : MovementStateBase
    {
        public IdleState(MovementStateMachine m) : base(m)
        {
        }

        public override void Enter() => _controller.Stop();

        public override void Execute()
        {
            if (!IsGrounded)
            {
                _machine.ChangeState<FallState>();
                return;
            }

            if (_controller.Input.IsDodging)
            {
                _machine.ChangeState<DodgeState>();
                return;
            }

            if (_controller.Input.IsJumping)
            {
                _machine.ChangeState<JumpState>();
                return;
            }

            if (InputDir.sqrMagnitude > .1f)
                _machine.ChangeState(_controller.Input.IsRunning ? typeof(RunState) : typeof(WalkState));
        }
    }
}
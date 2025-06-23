using Character.Movement.States.Base;
using Core.Events.Messages;

namespace Character.Movement.States
{
    public sealed class RunState : MovementStateBase
    {
        public RunState(MovementStateMachine m) : base(m) { }

        public override void Enter() => _bus.Publish(new PlayerStartedRunning());
        public override void Exit()  => _bus.Publish (new PlayerStoppedRunning());

        public override void Execute()
        {
            if (!IsGrounded)                         { _machine.ChangeState<FallState>(); return; }
            if (InputDir.sqrMagnitude < 0.1f)        { _machine.ChangeState<IdleState>(); return; }
            if (!_controller.Input.IsRunning)        { _machine.ChangeState<WalkState>(); return; }
            if (_controller.Input.IsDodging)         { _machine.ChangeState<DodgeState>(); return; }
            if (_controller.Input.IsJumping)         { _machine.ChangeState<JumpState>(); return; }

            _controller.Move  (InputDir, _controller.Config.runSpeed);
            _controller.Rotate(InputDir, _controller.Config.runRotationSpeed);
        }
    }
}
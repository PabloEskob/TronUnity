using Character.Movement.States.Base;
using Core.Events;

namespace Character.Movement.States
{
    public class RunState : MovementStateBase
    {
        public RunState(MovementStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            GameEvents.OnPlayerStartedRunning.Invoke();
        }

        public override void Execute()
        {
            var input = _controller.Input;
            var moveDirection = _controller.GetMovementDirection();

            // Check transitions
            if (moveDirection.magnitude < 0.1f)
            {
                _stateMachine.ChangeState<IdleState>();
                return;
            }

            if (!input.IsRunning)
            {
                _stateMachine.ChangeState<WalkState>();
                return;
            }

            if (input.IsDodging)
            {
                _stateMachine.ChangeState<DodgeState>();
                return;
            }

            if (input.IsJumping && _controller.Physics.IsGrounded)
            {
                _stateMachine.ChangeState<JumpState>();
                return;
            }

            // Movement
            _controller.Move(moveDirection, _controller.Config.runSpeed);
            _controller.Rotate(moveDirection, _controller.Config.runRotationSpeed);

            // Check if falling
            if (!_controller.Physics.IsGrounded)
            {
                _stateMachine.ChangeState<FallState>();
            }
        }

        public override void Exit()
        {
            GameEvents.OnPlayerStoppedRunning.Invoke();
        }
    }
}
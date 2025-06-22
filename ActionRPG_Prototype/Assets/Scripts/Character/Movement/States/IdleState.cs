using Character.Movement.States.Base;
using Core.Events;

namespace Character.Movement.States
{
    public class IdleState : MovementStateBase
    {
        public IdleState(MovementStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _controller.Stop();
        }

        public override void Execute()
        {
            var input = _controller.Input;

            // Check transitions
            if (input.MovementVector.magnitude > 0.1f)
            {
                if (input.IsRunning)
                {
                    _stateMachine.ChangeState<RunState>();
                }
                else
                {
                    _stateMachine.ChangeState<WalkState>();
                }

                return;
            }

            if (input.IsDodging && CanDodge())
            {
                _stateMachine.ChangeState<DodgeState>();
                return;
            }

            if (input.IsJumping && _controller.Physics.IsGrounded)
            {
                _stateMachine.ChangeState<JumpState>();
                return;
            }

            // Check if falling
            if (!_controller.Physics.IsGrounded)
            {
                _stateMachine.ChangeState<FallState>();
            }
        }

        private bool CanDodge()
        {
            // Add dodge cooldown logic here
            return true;
        }
    }
}
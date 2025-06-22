using Character.Movement.States.Base;

namespace Character.Movement.States
{
    public class FallState : MovementStateBase
    {
        public FallState(MovementStateMachine stateMachine) : base(stateMachine) { }

        public override void Execute()
        {
            // Allow limited movement in air
            var moveDirection = _controller.GetMovementDirection();
            if (moveDirection.magnitude > 0.1f)
            {
                var airSpeed = _controller.Config.walkSpeed * 0.5f;
                _controller.Move(moveDirection, airSpeed);
                _controller.Rotate(moveDirection, _controller.Config.walkRotationSpeed * 0.5f);
            }

            // Check if landed
            if (_controller.Physics.IsGrounded)
            {
                // Transition based on input
                var input = _controller.Input;
                if (input.MovementVector.magnitude > 0.1f)
                {
                    _stateMachine.ChangeState<WalkState>();
                }
                else
                {
                    _stateMachine.ChangeState<IdleState>();
                }
            }
        }
    }
}
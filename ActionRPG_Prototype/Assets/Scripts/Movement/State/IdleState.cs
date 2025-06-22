using CharacterMovement;
using Movement.Interface;

namespace Movement.State
{
    public class IdleState : MovementStateBase
    {
        private IMovementInput _input;

        public IdleState(CharacterMovementController controller, IMovementInput input)
            : base(controller)
        {
            _input = input;
        }

        public override void Execute()
        {
            // Проверяем ввод
            if (_input.MovementVector.magnitude > 0.1f)
            {
                if (_input.IsRunning)
                {
                    _controller.ChangeState<RunState>();
                }
                else
                {
                    _controller.ChangeState<WalkState>(); // ПЕРЕХОД В WALK!
                }

                return;
            }

            if (_input.IsDodging && _controller.CanDodge)
            {
                _controller.ChangeState<DodgeState>();
                return;
            }

            _controller.Stop();
        }
    }
}
// Movement States

using Assets.Scripts.Movement.Interface;
using CharacterMovement;

namespace Movement.State
{
    public abstract class MovementStateBase : IMovementState
    {
        protected CharacterMovementController _controller;

        public MovementStateBase(CharacterMovementController controller)
        {
            _controller = controller;
        }

        public virtual void Enter()
        {
        }

        public abstract void Execute();

        public virtual void Exit()
        {
        }
    }
}
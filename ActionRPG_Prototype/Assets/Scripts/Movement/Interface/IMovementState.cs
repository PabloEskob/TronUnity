namespace Movement.Interface
{
    public interface IMovementState
    {
        void Enter();
        void Execute();
        void FixedExecute(); // Для физических обновлений
        void Exit();
        bool CanTransitionTo<T>() where T : IMovementState;
    }
}
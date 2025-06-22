namespace Movement.Interface
{
    public interface IMovementState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}
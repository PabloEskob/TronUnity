namespace Core.Scripts.Infrastructure.States.StateInfrastructure
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}
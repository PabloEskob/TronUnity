using Core.Scripts.Infrastructure.States.StateInfrastructure;

namespace Core.Scripts.Infrastructure.States.StateMachine
{
    public interface IGameStateMachine
    {
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        TState GetState<TState>() where TState : class, IExitableState;
        TState ChangeState<TState>() where TState : class, IExitableState;
    }
}
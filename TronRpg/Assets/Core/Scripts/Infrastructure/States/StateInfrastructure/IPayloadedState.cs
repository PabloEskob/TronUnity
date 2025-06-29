namespace Core.Scripts.Infrastructure.States.StateInfrastructure
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
}
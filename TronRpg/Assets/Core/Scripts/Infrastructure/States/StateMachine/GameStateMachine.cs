using System;
using System.Collections.Generic;
using Core.Scripts.Infrastructure.States.StateInfrastructure;
using VContainer;

namespace Core.Scripts.Infrastructure.States.StateMachine
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly IObjectResolver _container;
        private readonly Dictionary<Type, IExitableState> _states = new();
        private IExitableState _activeState;

        public GameStateMachine(IObjectResolver container)
        {
            _container = container;
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }

        public TState GetState<TState>() where TState : class, IExitableState
        {
            var stateType = typeof(TState);
            if (!_states.TryGetValue(stateType, out var state))
            {
                state = _container.Resolve<TState>();
                _states[stateType] = state;
            }
            return state as TState;
        }

        public TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            var state = GetState<TState>();
            _activeState = state;
            return state;
        }
    }
}
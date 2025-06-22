using Movement.Interface;
using UnityEngine;

namespace Character.Movement.States.Base
{
    public abstract class MovementStateBase : IMovementState
    {
        protected readonly MovementStateMachine _stateMachine;
        protected readonly MovementController _controller;
        protected readonly Transform _transform;

        protected MovementStateBase(MovementStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _controller = stateMachine.Controller;
            _transform = stateMachine.transform;
        }

        public virtual void Enter() { }
        public abstract void Execute();
        public virtual void FixedExecute() { }
        public virtual void Exit() { }
        
        public virtual bool CanTransitionTo<T>() where T : IMovementState => true;
    }
}
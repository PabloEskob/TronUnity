using Movement.Interface;
using UnityEngine;

namespace Character.Movement.States.Base
{
    public abstract class MovementStateBase : IMovementState
    {
        protected readonly MovementStateMachine _machine;
        protected readonly MovementController   _controller;
        protected readonly Transform            _tf;

        protected MovementStateBase(MovementStateMachine m)
        {
            _machine    = m;
            _controller = m.Controller;
            _tf         = m.transform;
        }

        protected bool   IsGrounded => _controller.Physics.IsGrounded;
        protected Vector3 InputDir   => _controller.GetMovementDirection();

        public virtual void Enter() {}
        public abstract void Execute();
        public virtual void FixedExecute() {}
        public virtual void Exit() {}

        public virtual bool CanTransitionTo<T>() where T : IMovementState => typeof(T) != GetType();
    }
}
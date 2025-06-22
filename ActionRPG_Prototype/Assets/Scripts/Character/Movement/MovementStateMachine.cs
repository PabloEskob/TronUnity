using System;
using System.Collections.Generic;
using Character.Movement.States;
using Config.Movement;
using Movement.Interface;
using UnityEngine;

namespace Character.Movement
{
    public class MovementStateMachine : MonoBehaviour
    {
        private IMovementState _currentState;
        private Dictionary<Type, IMovementState> _states;

        public MovementController Controller { get; private set; }
        public MovementConfig Config { get; private set; }

        public Type CurrentStateType => _currentState?.GetType();

        private void Awake()
        {
            Controller = GetComponent<MovementController>();
            Config = Controller.Config;
            InitializeStates();
        }

        private void InitializeStates()
        {
            _states = new Dictionary<Type, IMovementState>
            {
                { typeof(IdleState), new IdleState(this) },
                { typeof(WalkState), new WalkState(this) },
                { typeof(RunState), new RunState(this) },
                { typeof(DodgeState), new DodgeState(this) },
                { typeof(JumpState), new JumpState(this) },
                { typeof(FallState), new FallState(this) }
            };
        }

        private void Start()
        {
            ChangeState<IdleState>();
        }

        private void Update()
        {
            _currentState?.Execute();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedExecute();
        }

        public void ChangeState<T>() where T : IMovementState
        {
            var targetType = typeof(T);

            if (_currentState != null && _currentState.GetType() == targetType)
                return;

            if (_currentState != null && !_currentState.CanTransitionTo<T>())
                return;

            if (!_states.TryGetValue(targetType, out var newState))
            {
                UnityEngine.Debug.LogError($"State {targetType.Name} not found!");
                return;
            }

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();

            UnityEngine.Debug.Log($"Changed state to: {targetType.Name}");
        }

        public bool IsInState<T>() where T : IMovementState
        {
            return _currentState != null && _currentState.GetType() == typeof(T);
        }
    }
}
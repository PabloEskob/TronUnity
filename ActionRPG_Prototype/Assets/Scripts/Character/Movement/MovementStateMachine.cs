using System;
using System.Collections.Generic;
using Character.Movement.States;
using Config.Movement;
using Movement.Interface;
using UnityEngine;

namespace Character.Movement
{
    public sealed class MovementStateMachine : MonoBehaviour
    {
        private readonly Dictionary<Type, IMovementState> _states = new();
        private IMovementState _current;

        public MovementController Controller { get; private set; }
        public Type CurrentStateType => _current?.GetType();

        private void Awake()
        {
            Controller = GetComponent<MovementController>();
            RegisterStates();
        }
        private void Start()        => ChangeState<IdleState>();
        private void Update()       => _current?.Execute();
        private void FixedUpdate()  => _current?.FixedExecute();

        // --- Public API ---
        public void ChangeState<T>() where T : IMovementState => ChangeState(typeof(T));

        public void ChangeState(Type targetType)
        {
            if (!_states.TryGetValue(targetType, out var next))
            {
                Debug.LogError($"Unregistered state {targetType}");
                return;
            }
            if (_current == next) return;
            if (_current != null && !_current.CanTransitionTo(targetType)) return; // extension method

            _current?.Exit();
            _current = next;
            _current.Enter();
        }

        public bool IsInState<T>() where T : IMovementState => _current?.GetType() == typeof(T);

        private void RegisterStates()
        {
            AddState(new IdleState(this));
            AddState(new WalkState(this));
            AddState(new RunState(this));
            AddState(new DodgeState(this));
            AddState(new JumpState(this));
            AddState(new FallState(this));
        }
        private void AddState(IMovementState s) => _states.Add(s.GetType(), s);
    }

    // ───────── Extension: generic‑>Type bridge ─────────
    public static class MovementStateExtensions
    {
        public static bool CanTransitionTo(this IMovementState state, Type targetType)
        {
            var m = state.GetType()
                          .GetMethod("CanTransitionTo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                          ?.MakeGenericMethod(targetType);
            if (m == null) return true; // default allow
            return (bool)m.Invoke(state, null);
        }
    }
}
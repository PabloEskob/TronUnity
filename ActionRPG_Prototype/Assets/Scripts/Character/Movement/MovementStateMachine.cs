using System;
using System.Collections.Generic;
using Character.Movement.States;
using Movement.Interface;
using UnityEngine;
using VContainer;
using Core.Events;

namespace Character.Movement
{
    [RequireComponent(typeof(MovementController))]
    public sealed class MovementStateMachine : MonoBehaviour
    {
        // ---------- Fields ----------
        readonly Dictionary<Type, IMovementState> _states = new();
        IMovementState _current;

        public MovementController Controller { get; private set; }
        public Type CurrentStateType => _current?.GetType();

        public IEventBus Bus { get; private set; } // будет проинжектировано

        // ---------- DI (method-injection) ----------
        [Inject] // вызовется VContainer ДО Awake
        public void Construct(IEventBus bus) => Bus = bus;

        // ---------- Unity lifecycle ----------
        void Awake()
        {
            Controller = GetComponent<MovementController>();
            RegisterStates(); // Bus и _states уже не null
        }

        void Start() => ChangeState<IdleState>();
        void Update() => _current?.Execute();
        void FixedUpdate() => _current?.FixedExecute();

        // ---------- Public API ----------
        public void ChangeState<T>() where T : IMovementState => ChangeState(typeof(T));

        public void ChangeState(Type targetType)
        {
            if (!_states.TryGetValue(targetType, out var next))
            {
                Debug.LogError($"Unregistered state {targetType}");
                return;
            }

            if (_current == next) return;
            if (_current != null && !_current.CanTransitionTo(targetType)) return;

            _current?.Exit();
            _current = next;
            _current.Enter();
        }

        public bool IsInState<T>() where T : IMovementState => _current?.GetType() == typeof(T);

        // ---------- Internal ----------
        void RegisterStates()
        {
            AddState(new IdleState(this));
            AddState(new WalkState(this));
            AddState(new RunState(this));
            AddState(new DodgeState(this));
            AddState(new JumpState(this));
            AddState(new FallState(this));
        }

        void AddState(IMovementState s) => _states.Add(s.GetType(), s);
    }

    // ───────── Extension: generic->Type bridge ─────────
    public static class MovementStateExtensions
    {
        public static bool CanTransitionTo(this IMovementState state, Type targetType)
        {
            var m = state.GetType()
                .GetMethod("CanTransitionTo",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance)
                ?.MakeGenericMethod(targetType);

            return m == null || (bool)m.Invoke(state, null);
        }
    }
}
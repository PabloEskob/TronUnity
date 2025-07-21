using Animancer;
using Animancer.TransitionLibraries;
using UnityEngine;
using System.Collections.Generic;
using Core.Scripts.Infrastructure.States.AnimationFactory;

namespace Core.Scripts.Character.Enemy
{
    public abstract class BaseEnemyAnimator : MonoBehaviour, IEnemyAnimator
    {
        [SerializeField] protected TransitionLibraryAsset IdleTransitionLibrary;
        [SerializeField] protected internal TransitionAsset WalkRunMixerTransition;
        [SerializeField] protected internal TransitionAsset AttackTransition;
        [SerializeField] protected internal TransitionAsset DeathTransition;
        [SerializeField] protected StringAsset SpeedParameterName;

        [Header("External")]
        [SerializeField] protected MonoBehaviour Controller; // Реализует IMovementProvider
        [SerializeField] protected AnimancerComponent Animancer;

        public EnemyState CurrentState { get; set; } = EnemyState.Idle;
        protected internal SmoothedFloatParameter SpeedParam { get; private set; }
        protected internal IMovementProvider MovementProvider { get; private set; }
        private TransitionLibrary _idleLibraryCache;

        private static readonly Dictionary<EnemyState, IAnimationState> StateMap = new()
        {
            { EnemyState.Idle, new IdleState() },
            { EnemyState.Walk, new WalkRunState() },
            { EnemyState.Run, new WalkRunState() },
            { EnemyState.Attack, new AttackState() },
            { EnemyState.Death, new DeathState() }
        };

        public enum EnemyState
        {
            Idle,
            Walk,
            Run,
            Attack,
            Death,
        }

        protected virtual void Awake()
        {
            if (!ValidateDependencies()) return;

            SpeedParam = new SmoothedFloatParameter(Animancer, SpeedParameterName, 0.05f);
            if (IdleTransitionLibrary != null)
            {
                _idleLibraryCache = IdleTransitionLibrary.Library;
            }

            InitializeSpecific();
            UpdateAnimationState(EnemyState.Idle);
        }

        protected abstract void InitializeSpecific();

        public virtual void UpdateAnimationState(EnemyState newState)
        {
            if (CurrentState == EnemyState.Death && newState != EnemyState.Death) return; // Нельзя выйти из Death

            if (newState != EnemyState.Idle && CurrentState == EnemyState.Idle)
            {
                var currentState = Animancer.Layers[0].CurrentState;
                if (currentState != null)
                {
                    currentState.Events(this).OnEnd = null;
                }
            }

            if (StateMap.TryGetValue(newState, out var state))
            {
                CurrentState = newState;
                state.Enter(this);
            }
            else
            {
                Debug.LogWarning($"No IAnimationState found for {newState}!");
            }
        }

        public AnimancerState PlayAnimation(ITransition transition)
        {
            if (transition == null)
            {
                Debug.LogWarning("Attempted to play null transition!");
                return null;
            }

            return Animancer.Play(transition);
        }

        protected internal void PlayRandomIdle()
        {
            if (_idleLibraryCache == null || _idleLibraryCache.Count <= 0)
            {
                Debug.LogWarning("IdleTransitionLibrary is empty or not initialized!");
                return;
            }

            if (CurrentState != EnemyState.Idle) return;

            int randomIndex = Random.Range(0, _idleLibraryCache.Count);
            if (_idleLibraryCache.TryGetTransition(randomIndex, out TransitionModifierGroup group))
            {
                if (group.Transition != null)
                {
                    var state = Animancer.Play(group.Transition);
                    state.Events(this).OnEnd ??= PlayRandomIdle;
                }
                else
                {
                    Debug.LogWarning("Received null Transition from group!");
                }
            }
            else
            {
                Debug.LogWarning("Failed to get TransitionModifierGroup by index!");
            }
        }

        private bool ValidateDependencies()
        {
            if (Animancer == null || SpeedParameterName == null)
            {
                Debug.LogError("Animancer or SpeedParameterName not set!");
                enabled = false;
                return false;
            }

            MovementProvider = Controller as IMovementProvider;
            if (MovementProvider == null)
            {
                Debug.LogError("Controller must implement IMovementProvider!");
                enabled = false;
                return false;
            }

            return true;
        }
    }
}
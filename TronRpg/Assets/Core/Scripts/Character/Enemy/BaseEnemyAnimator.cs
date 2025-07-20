using Animancer;
using Animancer.TransitionLibraries;
using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public abstract class BaseEnemyAnimator : MonoBehaviour, IEnemyAnimator
    {
        [SerializeField] protected TransitionLibraryAsset IdleTransitionLibrary;
        [SerializeField] protected TransitionAsset WalkRunMixerTransition;
        [SerializeField] protected TransitionAsset AttackTransition;
        [SerializeField] protected TransitionAsset DeathTransition;
        [SerializeField] protected StringAsset SpeedParameterName;

        [Header("External")]
        [SerializeField] protected ECM2.Character Controller;
        [SerializeField] protected AnimancerComponent Animancer;

        protected EnemyState CurrentState = EnemyState.Idle;
        private SmoothedFloatParameter _speedParam;
        private TransitionLibrary _idleLibraryCache;

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
            _speedParam = new SmoothedFloatParameter(Animancer, SpeedParameterName, 0.05f);
            if (IdleTransitionLibrary != null)
            {
                _idleLibraryCache = IdleTransitionLibrary.Library; 
            }
            InitializeSpecific();
            UpdateAnimationState(EnemyState.Idle);
        }

        protected abstract void InitializeSpecific();

        protected virtual void Update()
        {
            UpdateSpeedParameter();
        }

        protected void UpdateSpeedParameter()
        {
            if (CurrentState == EnemyState.Walk || CurrentState == EnemyState.Run)
            {
                var speed = Controller.velocity.magnitude;
                var normalized = Mathf.InverseLerp(0f, Controller.speed, speed);
                _speedParam.TargetValue = normalized;
            }
        }

        public virtual void UpdateAnimationState(EnemyState newState)
        {
            if (newState != EnemyState.Idle && CurrentState == EnemyState.Idle)
            {
                var currentState = Animancer.Layers[0].CurrentState;
                if (currentState != null)
                {
                    currentState.Events(this).OnEnd = null;
                }
            }

            CurrentState = newState;
            switch (newState)
            {
                case EnemyState.Idle:
                    PlayRandomIdle();
                    break;
                case EnemyState.Walk:
                case EnemyState.Run:
                    PlayAnimation(WalkRunMixerTransition);
                    break;
                case EnemyState.Attack:
                    PlayAnimation(AttackTransition);
                    break;
                case EnemyState.Death:
                    PlayAnimation(DeathTransition);
                    break;
            }
        }

        public AnimancerState PlayAnimation(TransitionAsset transition)
        {
            return Animancer.Play(transition);
        }

        private void PlayRandomIdle()
        {
            if (_idleLibraryCache == null || _idleLibraryCache.Count <= 0)
            {
                Debug.LogWarning("IdleTransitionLibrary пуста или не инициализирована!");
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
                    Debug.LogWarning("Получен null Transition из группы!");
                }
            }
            else
            {
                Debug.LogWarning("Не удалось получить TransitionModifierGroup по индексу!");
            }
        }
    }
}
using Core.Scripts.Character.Enemy.Interface;
using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public abstract class BaseMovementHandler : MonoBehaviour
    {
        [SerializeField] private FollowerEntityAdapter _movementProviderComponent;
        [SerializeField] private BaseEnemyAnimator _animatorComponent;

        protected FollowerEntityAdapter MovementProvider { get; private set; }
        protected BaseEnemyAnimator Animator { get; private set; }

        protected virtual void Awake()
        {
            MovementProvider = _movementProviderComponent;
            Animator = _animatorComponent;
            SubscribeToEvents();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        protected abstract void SubscribeToEvents();
        protected abstract void UnsubscribeFromEvents();
    }
}
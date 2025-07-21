using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public abstract class BaseMovementHandler : MonoBehaviour
    {
        [SerializeField] private FollowerEntityAdapter _movementProviderComponent; 
        [SerializeField] private BaseEnemyAnimator _animatorComponent;

        protected IMovementProvider MovementProvider { get; private set; }
        protected IEnemyAnimator Animator { get; private set; }

        protected virtual void Awake()
        {
            MovementProvider = _movementProviderComponent as IMovementProvider ?? GetComponent<IMovementProvider>();
            Animator = _animatorComponent as IEnemyAnimator ?? GetComponent<IEnemyAnimator>();

            if (MovementProvider == null || Animator == null)
            {
                Debug.LogError($"{GetType().Name}: Missing IMovementProvider or IEnemyAnimator!");
                enabled = false;
                return;
            }

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
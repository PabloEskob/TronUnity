using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class PathCompletionHandler : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _movementProviderComponent; // Реализует IMovementProvider
        [SerializeField] private MonoBehaviour _animatorComponent; // Реализует IEnemyAnimator

        private IMovementProvider _movementProvider;
        private IEnemyAnimator _animator;

        private void Awake()
        {
            _movementProvider = _movementProviderComponent as IMovementProvider ?? GetComponent<IMovementProvider>();
            _animator = _animatorComponent as IEnemyAnimator ?? GetComponent<IEnemyAnimator>();

            if (_movementProvider == null || _animator == null)
            {
                Debug.LogError("PathCompletionHandler: Missing IMovementProvider or IEnemyAnimator!");
                enabled = false;
                return;
            }

            _movementProvider.OnPathCompleted += HandlePathCompleted; // Подписка на событие
        }

        private void OnDestroy()
        {
            if (_movementProvider != null)
            {
                _movementProvider.OnPathCompleted -= HandlePathCompleted; // Отписка для избежания leaks
            }
        }

        private void HandlePathCompleted()
        {
            if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Death) return;

            if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Walk ||
                _animator.CurrentState == BaseEnemyAnimator.EnemyState.Run)
            {
                _animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Idle);
                Debug.Log("Target reached.");
            }
        }
    }
}
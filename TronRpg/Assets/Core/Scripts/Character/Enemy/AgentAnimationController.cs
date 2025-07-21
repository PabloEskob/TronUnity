using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class AgentAnimationController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _movementProviderComponent; // Реализует IMovementProvider
        [SerializeField] private MonoBehaviour _animatorComponent; // Реализует IEnemyAnimator
        [SerializeField] private float _moveThreshold = 0.1f; // Порог для начала движения
        [SerializeField] private float _runThreshold = 0.5f; // Порог normalized speed для Run vs Walk

        private IMovementProvider _movementProvider;
        private IEnemyAnimator _animator;
        private float _cachedMaxSpeed; // Кэш для оптимизации

        private void Awake()
        {
            _movementProvider = _movementProviderComponent as IMovementProvider ?? GetComponent<IMovementProvider>();
            _animator = _animatorComponent as IEnemyAnimator ?? GetComponent<IEnemyAnimator>();

            if (_movementProvider == null || _animator == null)
            {
                Debug.LogError("AgentAnimationController: Missing IMovementProvider or IEnemyAnimator!");
                enabled = false;
                return;
            }

            _cachedMaxSpeed = _movementProvider.MaxSpeed;
            _movementProvider.OnVelocityChanged += HandleVelocityChanged; // Подписка
        }

        private void OnDestroy()
        {
            if (_movementProvider != null)
            {
                _movementProvider.OnVelocityChanged -= HandleVelocityChanged; // Отписка
            }
        }

        private void HandleVelocityChanged(Vector3 newVelocity)
        {
            if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Death) return;

            var speed = newVelocity.magnitude;
            var normalizedSpeed = Mathf.InverseLerp(0f, _cachedMaxSpeed, speed);

            var newState = speed > _moveThreshold
                ? (normalizedSpeed >= _runThreshold ? BaseEnemyAnimator.EnemyState.Run : BaseEnemyAnimator.EnemyState.Walk)
                : BaseEnemyAnimator.EnemyState.Idle;

            if (newState != _animator.CurrentState)
            {
                _animator.UpdateAnimationState(newState);
            }
        }
    }
}
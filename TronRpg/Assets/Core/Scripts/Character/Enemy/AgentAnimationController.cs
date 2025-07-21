using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class AgentAnimationController : BaseMovementHandler
    {
        [SerializeField] private float _moveThreshold = 0.1f; // Порог для начала движения
        [SerializeField] private float _runThreshold = 0.5f; // Порог normalized speed для Run vs Walk
        
        private float _cachedMaxSpeed; // Кэш для оптимизации

        protected override void Awake()
        {
            base.Awake();
            if (!enabled) return;
            _cachedMaxSpeed = MovementProvider.MaxSpeed;
        }

        protected override void SubscribeToEvents()
        {
            MovementProvider.OnVelocityChanged += HandleVelocityChanged;
        }

        protected override void UnsubscribeFromEvents()
        {
            MovementProvider.OnVelocityChanged -= HandleVelocityChanged;
        }

        private void HandleVelocityChanged(Vector3 newVelocity)
        {
            if (Animator.CurrentState == BaseEnemyAnimator.EnemyState.Death) return;

            var speed = newVelocity.magnitude;
            var normalizedSpeed = Mathf.InverseLerp(0f, _cachedMaxSpeed, speed);

            var newState = speed > _moveThreshold
                ? (normalizedSpeed >= _runThreshold ? BaseEnemyAnimator.EnemyState.Run : BaseEnemyAnimator.EnemyState.Walk)
                : BaseEnemyAnimator.EnemyState.Idle;

            if (newState != Animator.CurrentState)
            {
                Animator.UpdateAnimationState(newState);
            }
        }
    }
}
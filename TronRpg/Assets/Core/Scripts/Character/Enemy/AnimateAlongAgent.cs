using UnityEngine;
using Pathfinding;

namespace Core.Scripts.Character.Enemy
{
    public class AnimateAlongAgent : MonoBehaviour
    {
        [SerializeField] private FollowerEntity _agent; // A* агент
        [SerializeField] private BaseEnemyAnimator _animator; // Через IEnemyAnimator для DIP
        [SerializeField] private float _moveThreshold = 0.1f; // Threshold для начала/окончания
        [SerializeField] private float _runThreshold = 0.5f; // Normalized speed для Run vs Walk

        private bool _wasReached = true; // Флаг для имитации one-time события (SRP)

        private void Awake()
        {
            if (_agent == null) _agent = GetComponent<FollowerEntity>();
            if (_animator == null) _animator = GetComponent<BaseEnemyAnimator>();
            if (_agent == null || _animator == null)
            {
                Debug.LogError("AnimateAlongAgent: Missing dependencies!");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Death) return; // Игнор если мертв

            var speed = _agent.velocity.magnitude;
            var normalizedSpeed = Mathf.InverseLerp(0f, _agent.maxSpeed, speed);

            if (speed > _moveThreshold)
            {
                // Начало движения
                if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Idle ||
                    _animator.CurrentState == BaseEnemyAnimator.EnemyState.Attack)
                {
                    var newState = normalizedSpeed >= _runThreshold
                        ? BaseEnemyAnimator.EnemyState.Run
                        : BaseEnemyAnimator.EnemyState.Walk;
                    _animator.UpdateAnimationState(newState);
                    _wasReached = false; // Сброс флага
                }
                else if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Walk && normalizedSpeed >= _runThreshold)
                {
                    _animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Run);
                }
                else if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Run && normalizedSpeed < _runThreshold)
                {
                    _animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Walk);
                }
            }
            else
            {
                // Окончание движения (velocity-based)
                if (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Walk ||
                    _animator.CurrentState == BaseEnemyAnimator.EnemyState.Run)
                {
                    _animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Idle);
                }
            }

            // Дополнительная проверка reachedEndOfPath для точности (имитация события)
            if (_agent.reachedEndOfPath && !_wasReached &&
                (_animator.CurrentState == BaseEnemyAnimator.EnemyState.Walk ||
                 _animator.CurrentState == BaseEnemyAnimator.EnemyState.Run))
            {
                _animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Idle);
                _wasReached = true;
                Debug.Log("Target reached via reachedEndOfPath.");
            }
        }
    }
}
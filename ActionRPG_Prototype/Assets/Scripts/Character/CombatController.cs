// CombatController.cs

using UnityEngine;

namespace Character
{
    public class CombatController : MonoBehaviour
    {
        [Header("Combat Settings")] [SerializeField]
        private float _attackRange = 2f;

        [SerializeField] private float _attackAngle = 60f;
        [SerializeField] private LayerMask _enemyLayer;

        [Header("References")] [SerializeField]
        private Transform _weaponSlot;

        [SerializeField] private CharacterAnimationController _animationController;

        private int _currentComboIndex = 0;
        private float _lastAttackTime;
        private float _comboResetTime = 1.5f;

        private void Awake()
        {
            if (_animationController != null)
            {
                _animationController.OnAttackHit += PerformAttackHit;
            }
        }

        public void Attack()
        {
            if (Time.time - _lastAttackTime > _comboResetTime)
            {
                _currentComboIndex = 0;
            }

            _animationController.TriggerAttack(_currentComboIndex);
            _lastAttackTime = Time.time;

            _currentComboIndex = (_currentComboIndex + 1) % 3; // 3-hit combo
        }

        private void PerformAttackHit()
        {
            // Проверка попаданий по врагам
            Collider[] hits = Physics.OverlapSphere(transform.position, _attackRange, _enemyLayer);

            foreach (var hit in hits)
            {
                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToTarget);

                if (angle < _attackAngle / 2f)
                {
                    // Нанести урон
                    UnityEngine.Debug.Log($"Hit: {hit.name}");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
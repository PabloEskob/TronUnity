// CombatController.cs

using Character.Animation;
using Character.Stats;
using UnityEngine;

namespace Character.Combat
{
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(CharacterStats))]
    public sealed class CombatController : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float _attackStaminaCost = 25f;
        [SerializeField] private float _attackCooldown    = 0.5f;

        private AnimationController _anim;
        private CharacterStats      _stats;
        private float _nextAttack;
        private int   _comboIndex;

        private void Awake()
        {
            _anim  = GetComponent<AnimationController>();
            _stats = GetComponent<CharacterStats>();
        }

        public void TryAttack()
        {
            if (Time.time < _nextAttack) return;
            if (_stats.CurrentStamina < _attackStaminaCost) return;

            _stats.SpendStamina(_attackStaminaCost);
            _anim.PlayAttackAnimation(_comboIndex);
            _comboIndex = (_comboIndex + 1) % 3; // simple 3‑hit combo
            _nextAttack = Time.time + _attackCooldown;
        }
    }
}


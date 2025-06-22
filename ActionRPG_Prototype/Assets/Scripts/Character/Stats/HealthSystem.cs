using System;
using Character.Core;
using Character.Stats;
using Core.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Character.Core
{
    public sealed class HealthSystem : MonoBehaviour
    {
        [System.Serializable] public class DamageEvent : UnityEvent<float,float>{}
        [System.Serializable] public class DeathEvent  : UnityEvent{}

        [SerializeField] private CharacterStats _stats;
        [SerializeField] private float _invincibilityTime = 0.3f;

        public DamageEvent OnDamaged = new();
        public DeathEvent  OnDied    = new();

        private float _lastHit;
        public bool  IsDead => _stats.CurrentHealth <= 0;
        public float Health => _stats.CurrentHealth;

        private void Awake() { if (_stats == null) _stats = GetComponent<CharacterStats>(); }

        public void InitializeFromConfig(CharacterConfig cfg) => _stats.InitializeFromConfig(cfg);

        public void DealDamage(float amount)
        {
            if (Time.time < _lastHit + _invincibilityTime || IsDead) return;
            _lastHit = Time.time;
            float old = _stats.CurrentHealth;
            _stats.CurrentHealth = Mathf.Max(_stats.CurrentHealth - amount, 0);
            OnDamaged.Invoke(old, _stats.CurrentHealth);
            if (IsDead) OnDied.Invoke();
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            float old = _stats.CurrentHealth;
            _stats.CurrentHealth = Mathf.Min(_stats.CurrentHealth + amount, _stats.MaxHealth);
            OnDamaged.Invoke(old, _stats.CurrentHealth);
        }
    }
}

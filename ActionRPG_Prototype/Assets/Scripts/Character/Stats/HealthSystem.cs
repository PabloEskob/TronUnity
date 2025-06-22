using System;
using Character.Core;
using Core.Events;
using UnityEngine;

namespace Character.Stats
{
    public class HealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _maxHealth = 100f;
        
        [Header("Damage Settings")]
        [SerializeField] private float _invincibilityDuration = 0.5f;
        
        private float _lastDamageTime;
        private CharacterStats _stats;
        
        // Events
        public event Action<float> OnHealthChanged;
        public event Action<float> OnDamaged;
        public event Action OnDeath;
        
        // Properties
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float HealthPercentage => _currentHealth / _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        public bool IsInvincible => Time.time - _lastDamageTime < _invincibilityDuration;
        
        private void Awake()
        {
            _stats = GetComponent<CharacterStats>();
            _currentHealth = _maxHealth;
        }

        public void InitializeFromConfig(CharacterConfig config)
        {
            _maxHealth = config.maxHealth;
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float damage, GameObject attacker = null)
        {
            if (!IsAlive || IsInvincible) return;
            
            var actualDamage = _stats.CalculateDefense(damage);
            _currentHealth = Mathf.Max(0, _currentHealth - actualDamage);
            _lastDamageTime = Time.time;
            
            OnHealthChanged?.Invoke(_currentHealth);
            OnDamaged?.Invoke(actualDamage);
            
            if (CompareTag("Player"))
            {
                GameEvents.OnPlayerDamaged.Invoke(actualDamage);
            }
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            
            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth);
        }

        public void SetHealth(float health)
        {
            _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth);
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDeath?.Invoke();
            // Handle death logic
        }
    }
}
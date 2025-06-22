using Character.Core;
using UnityEngine;

namespace Character.Stats
{
    public class CharacterStats : MonoBehaviour
    {
        [Header("Base Stats")] [SerializeField]
        private float _maxHealth = 100f;

        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _defense = 10f;

        [Header("Combat Stats")] [SerializeField]
        private float _attackPower = 10f;

        [SerializeField] private float _criticalChance = 0.1f;
        [SerializeField] private float _criticalMultiplier = 2f;

        [Header("Movement Stats")] [SerializeField]
        private float _moveSpeedMultiplier = 1f;

        // Current values
        private float _currentStamina;

        // Properties
        public float MaxHealth => _maxHealth;
        public float MaxStamina => _maxStamina;
        public float Defense => _defense;
        public float AttackPower => _attackPower;
        public float CriticalChance => _criticalChance;
        public float CriticalMultiplier => _criticalMultiplier;
        public float CurrentStamina => _currentStamina;

        public float StaminaPercentage => _currentStamina / _maxStamina;

        private void Awake()
        {
            _currentStamina = _maxStamina;
        }

        public void InitializeFromConfig(CharacterConfig config)
        {
            _maxHealth = config.maxHealth;
            _maxStamina = config.maxStamina;
            _defense = config.defense;
            _attackPower = config.baseDamage;
            _criticalChance = config.criticalChance;
            _criticalMultiplier = config.criticalMultiplier;

            _currentStamina = _maxStamina;
        }

        public bool ConsumeStamina(float amount)
        {
            if (_currentStamina >= amount)
            {
                _currentStamina -= amount;
                return true;
            }

            return false;
        }

        public void RegenerateStamina(float amount)
        {
            _currentStamina = Mathf.Min(_currentStamina + amount, _maxStamina);
        }

        public float CalculateDamage(float baseDamage, bool isCritical = false)
        {
            var damage = baseDamage * _attackPower;

            if (isCritical || Random.value < _criticalChance)
            {
                damage *= _criticalMultiplier;
            }

            return damage;
        }

        public float CalculateDefense(float incomingDamage)
        {
            return Mathf.Max(1f, incomingDamage - _defense);
        }
    }
}
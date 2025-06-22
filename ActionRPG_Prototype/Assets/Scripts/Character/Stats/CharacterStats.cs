using Character.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Character.Stats
{
    public sealed class CharacterStats : MonoBehaviour
    {
        [System.Serializable] public class StatChangedEvent : UnityEvent<float,float>{}

        [SerializeField] private float _maxHealth   = 100;
        [SerializeField] private float _maxStamina  = 100;
        [SerializeField] private float _staminaRegenPerSec = 15;

        public float MaxHealth      { get; private set; }
        public float MaxStamina     { get; private set; }
        public float CurrentHealth  { get; internal set; }  // <-- internal setter для HealthSystem
        public float CurrentStamina { get; private set; }

        public StatChangedEvent OnHealthChanged  = new();
        public StatChangedEvent OnStaminaChanged = new();

        private void Awake()
        {
            MaxHealth      = _maxHealth;
            MaxStamina     = _maxStamina;
            CurrentHealth  = MaxHealth;
            CurrentStamina = MaxStamina;
        }

        private void Update() => RegenerateStamina();

        public void InitializeFromConfig(CharacterConfig cfg)
        {
            MaxHealth   = cfg.baseHealth;
            MaxStamina  = cfg.baseStamina;
            CurrentHealth  = MaxHealth;
            CurrentStamina = MaxStamina;
        }

        public void SpendStamina(float cost)
        {
            if (CurrentStamina < cost) return;
            float old = CurrentStamina;
            CurrentStamina -= cost;
            OnStaminaChanged.Invoke(old, CurrentStamina);
        }

        public void RestoreStamina(float amount)
        {
            float old = CurrentStamina;
            CurrentStamina = Mathf.Min(CurrentStamina + amount, MaxStamina);
            OnStaminaChanged.Invoke(old, CurrentStamina);
        }

        private void RegenerateStamina() => RestoreStamina(_staminaRegenPerSec * Time.deltaTime);
    }
}
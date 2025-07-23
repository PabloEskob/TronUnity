using System;
using Core.Scripts.Character.Interface;
using Core.Scripts.Data;
using Core.Scripts.Services.PersistentProgress;
using UnityEngine;

namespace Core.Scripts.Character.Hero
{
    public class HeroHealth : MonoBehaviour, ISavedProgress,IDamageable
    {
        private State _state;
        
        public event Action<float> OnDamageTaken;

        public float Current
        {
            get => _state.CurrentHealth;
            set => _state.CurrentHealth = value;
        }

        public float Max
        {
            get => _state.MaxHealth;
            set => _state.MaxHealth = value;
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            _state = playerProgress.HeroState;
        }

        public void UpdateProgress(PlayerProgress playerProgress)
        {
            playerProgress.HeroState.CurrentHealth = Current;
            playerProgress.HeroState.MaxHealth = Max;
        }

        public void TakeDamage(float damage)
        {
            if (Current <= 0) return;
            Current -= damage;
            OnDamageTaken?.Invoke(damage);
        }
    }
}
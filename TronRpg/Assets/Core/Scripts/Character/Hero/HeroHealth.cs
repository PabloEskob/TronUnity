using System;
using Core.Scripts.Character.Animator;
using Core.Scripts.Character.Interface;
using Core.Scripts.Data;
using Core.Scripts.Services.PersistentProgress;
using UnityEngine;

namespace Core.Scripts.Character.Hero
{
    public class HeroHealth : MonoBehaviour, ISavedProgress, IDamageable
    {
        public HeroAnimator Animator;
        private State _state;

        public Action HealthChanged;

        public float Current
        {
            get => _state.CurrentHealth;
            set
            {
                if (!Mathf.Approximately(_state.CurrentHealth, value))
                {
                    _state.CurrentHealth = value;
                    HealthChanged?.Invoke();
                }
            }
        }

        public float Max
        {
            get => _state.MaxHealth;
            set => _state.MaxHealth = value;
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            _state = playerProgress.HeroState;
            HealthChanged?.Invoke();
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
            if (Current <= 0) return;
            Animator.PlayHit();
            
        }
    }
}
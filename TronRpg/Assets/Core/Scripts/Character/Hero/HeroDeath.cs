
using Core.Scripts.Character.Animator;
using UnityEngine;

namespace Core.Scripts.Character.Hero
{
    public class HeroDeath : MonoBehaviour
    {
        
        public HeroHealth HeroHealth;
        public HeroMove Move;
        public GameObject DeathEffect;

        private bool _isDead;

        private void Start() =>
            HeroHealth.HealthChanged += HealthChanged;

        private void OnDestroy() =>
            HeroHealth.HealthChanged -= HealthChanged;

        private void HealthChanged()
        {
            if (!_isDead && HeroHealth.Current <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;
            Move.StopMovement();
            Instantiate(DeathEffect, transform.position, Quaternion.identity);
        }
    }
}
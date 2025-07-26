using Animancer;
using Core.Scripts.Character.Animator;
using UnityEngine;

namespace Core.Scripts.Character.Hero
{
    public class HeroDeath : MonoBehaviour
    {
        [SerializeField] private TransitionAsset DeathTransition;

        public HeroHealth HeroHealth;
        public HeroMove Move;
        public HeroAnimator Animancer;
        public GameObject DeathEffect;
        public UnityEngine.Animator Animator;

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
            Animator.applyRootMotion = true;
            Animancer.PlayTransition(DeathTransition);
            Instantiate(DeathEffect, transform.position, Quaternion.identity);
        }
    }
}
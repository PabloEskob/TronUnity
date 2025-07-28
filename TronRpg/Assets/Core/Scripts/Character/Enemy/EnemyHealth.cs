using System;
using Animancer;
using Core.Scripts.Character.Interface;
using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class EnemyHealth : MonoBehaviour, IHealth
    {
        public BaseEnemyAnimator Animator;
        public ClipTransition Hit;

        [SerializeField] private float _current;
        [SerializeField] private float _max;

        public event Action HealthChanged;

        public float Current
        {
            get => _current;
            set => _current = value;
        }

        public float Max
        {
            get => _max;
            set => _max = value;
        }

        public void TakeDamage(float damage)
        {
            Current -= damage;
            Animator.PlayHit(Hit);
            HealthChanged?.Invoke();
        }
    }
}
using System;
using System.Collections;

using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class EnemyDeath : MonoBehaviour
    {
        public EnemyHealth Health;

        public GameObject DeathFx;

       

        public event Action Happened;

        private void Start()
        {
            Health.HealthChanged += HealthChanged;
        }

        private void OnDestroy()
        {
            Health.HealthChanged -= HealthChanged;
        }

        private void HealthChanged()
        {
            if (Health.Current <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Health.HealthChanged -= HealthChanged;

           

            SpawnDeathFx();
            StartCoroutine(DestroyTimer());

            Happened?.Invoke();
        }

        private void SpawnDeathFx()
        {
            if (DeathFx)
                Instantiate(DeathFx, transform.position, Quaternion.identity);
        }

        private IEnumerator DestroyTimer()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }
}
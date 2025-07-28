using System;
using Core.Scripts.Character.Interface;
using UnityEngine;

namespace Core.Scripts.UI
{
    public class ActorUI : MonoBehaviour
    {
        public HpBar HpBar;
        private IHealth _heroHealth;

        public void Construct(IHealth heroHealth)
        {
            _heroHealth = heroHealth;
            _heroHealth.HealthChanged += UpdateHpBar;
        }

        private void Start()
        {
            var health = GetComponent<IHealth>();
            if (health != null)
                Construct(health);
        }

        private void OnDestroy() =>
            _heroHealth.HealthChanged -= UpdateHpBar;

        private void UpdateHpBar()
        {
            HpBar.SetValue(_heroHealth.Current, _heroHealth.Max);
        }
    }
}
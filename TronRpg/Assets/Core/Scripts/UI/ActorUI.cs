using Core.Scripts.Character.Hero;
using UnityEngine;

namespace Core.Scripts.UI
{
    public class ActorUI : MonoBehaviour
    {
        public HpBar HpBar;
        private HeroHealth _heroHealth;

        private void OnDestroy() => 
            _heroHealth.HealthChanged -= UpdateHpBar;

        public void Construct(HeroHealth heroHealth)
        {
            _heroHealth = heroHealth;
            _heroHealth.HealthChanged += UpdateHpBar;
        }

        private void UpdateHpBar()
        {
            HpBar.SetValue(_heroHealth.Current, _heroHealth.Max);
        }
    }
}
using System;

namespace Core.Scripts.Data
{
    [Serializable]
    public class State
    {
        public float CurrentHealth;
        public float MaxHealth;

        public void ResetHP() => CurrentHealth = MaxHealth;
    }
}
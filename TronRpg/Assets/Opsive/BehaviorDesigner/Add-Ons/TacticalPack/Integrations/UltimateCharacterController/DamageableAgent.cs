/// ---------------------------------------------
/// Tactical Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.TacticalPack.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.AddOns.TacticalPack.Runtime;
    using Opsive.UltimateCharacterController.Traits;
    using UnityEngine;

    /// <summary>
    /// Implements IDamageable for the Ultimate Character Controller Health component.
    /// </summary>
    public class DamageableAgent : MonoBehaviour, IDamageable
    {
        /// <summary>
        /// Is the object currently alive?
        /// </summary>
        public bool IsAlive => m_Health.IsAlive();

        private Health m_Health;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_Health = GetComponent<Health>();
        }

        /// <summary>
        /// Take damage by the specified amount.
        /// </summary>
        /// <param name="amount">The amount of damage to take.</param>
        public void Damage(float amount)
        {
            // Intentionally left blank - the character controller handles damage.
        }
    }
}
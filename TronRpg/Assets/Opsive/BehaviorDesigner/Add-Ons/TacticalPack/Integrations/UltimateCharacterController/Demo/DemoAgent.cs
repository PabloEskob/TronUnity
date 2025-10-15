/// ---------------------------------------------
/// Tactical Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.TacticalPack.Integrations.UltimateCharacterController.Demo
{
    using Opsive.Shared.StateSystem;
    using UnityEngine;

    /// <summary>
    /// Sets up the Tactical Pack / Ultimate Character Controller demo agent.
    /// </summary>
    public class DemoAgent : MonoBehaviour
    {
        /// <summary>
        /// Sets the correct states.
        /// </summary>
        public void Start()
        {
            StateManager.SetState(gameObject, "AssaultRifle_Trigger_Repeat", false);
            StateManager.SetState(gameObject, "AssaultRifle_Trigger_Simple", true);
        }
    }
}
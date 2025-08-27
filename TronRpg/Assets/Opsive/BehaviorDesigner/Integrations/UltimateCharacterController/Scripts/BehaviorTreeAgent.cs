/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.Runtime;
    using Opsive.Shared.Events;
    using Opsive.Shared.StateSystem;
    using UnityEngine;

    /// <summary>
    /// Automatically disables/enables the behavior tree when the character dies/respawns.
    /// </summary>
    public class BehaviorTreeAgent : StateBehavior
    {
        [Tooltip("When the agent dies should the behavior tree be paused instead of disabled?")]
        [SerializeField] protected bool m_PauseOnDeath = false;

        private BehaviorTree m_BehaviorTree;
        private bool m_BehaviorTreeActive;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_BehaviorTree = GetComponent<BehaviorTree>();

            if (m_BehaviorTree != null) {
                EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(gameObject, "OnDeath", OnDeath);
                EventHandler.RegisterEvent(gameObject, "OnRespawn", OnRespawn);
            }
        }

        /// <summary>
        /// The character has died.
        /// </summary>
        /// <param name="position">The position of the force.</param>
        /// <param name="force">The amount of force which killed the character.</param>
        /// <param name="attacker">The GameObject that killed the character.</param>
        private void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            m_BehaviorTreeActive = m_BehaviorTree.IsActive();
            if (m_BehaviorTreeActive) {
                m_BehaviorTree.StopBehavior(m_PauseOnDeath);
            }
        }

        /// <summary>
        /// The character has respawned.
        /// </summary>
        private void OnRespawn()
        {
            if (m_BehaviorTreeActive) {
                m_BehaviorTree.StartBehavior();
                m_BehaviorTreeActive = false;
            }
        }

        /// <summary>
        /// The GameObject was destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(gameObject, "OnDeath", OnDeath);
            EventHandler.UnregisterEvent(gameObject, "OnRespawn", OnRespawn);
        }
    }
}
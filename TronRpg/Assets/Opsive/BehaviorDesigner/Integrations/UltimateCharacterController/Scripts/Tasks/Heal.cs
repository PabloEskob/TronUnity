/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.Shared.Game;
    using Opsive.UltimateCharacterController.Traits;
    using UnityEngine;

    [NodeDescription("Heals the agent.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class Heal : TargetGameObjectAction
    {
        [Tooltip("The amount to heal the agent by.")]
        public SharedVariable<float> m_Amount;

        private GameObject m_PrevTarget;
        private Health m_Health;

        /// <summary>
        /// Retrieves the health component.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_Health = gameObject.GetCachedComponent<Health>();
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Returns succes if the agent is healed.
        /// </summary>
        /// <returns>Success if the agent is healed.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Health == null || m_Amount.Value < 0) {
                return TaskStatus.Failure;
            }

            m_Health.Heal(m_Amount.Value);
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_Amount = 0;
        }
    }
}
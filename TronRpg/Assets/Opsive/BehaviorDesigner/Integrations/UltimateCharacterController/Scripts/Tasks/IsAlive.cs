/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.Shared.Game;
    using Opsive.UltimateCharacterController.Traits;
    using UnityEngine;

    [NodeDescription("Is the agent alive?")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class IsAlive : TargetGameObjectConditional
    {
        private GameObject m_PrevTarget;
        private Health m_Health;

        /// <summary>
        /// Retrieves the health component.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_Health = gameObject.GetCachedParentComponent<Health>();
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Returns succes if the agent is alive.
        /// </summary>
        /// <returns>Success if the agent is alive.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Health == null) {
                return TaskStatus.Failure;
            }

            return m_Health.IsAlive() ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
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
    using Opsive.Shared.Events;
    using UnityEngine;

    [NodeDescription("Executes the event using the Ultimate Character Controller event system.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class ExecuteEvent : TargetGameObjectAction
    {
        [Tooltip("The event name to send.")]
        public SharedVariable<string> m_EventName;

        /// <summary>
        /// Executes the specified event.
        /// </summary>
        /// <returns>Success if the event was successfully executed.</returns>
        public override TaskStatus OnUpdate()
        {
            if (string.IsNullOrEmpty(m_EventName.Value)) {
                return TaskStatus.Failure;
            }

            EventHandler.ExecuteEvent(gameObject, m_EventName.Value);
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_EventName = string.Empty;
        }
    }
}
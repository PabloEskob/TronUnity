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
    using Opsive.Shared.StateSystem;
    using UnityEngine;

    [NodeDescription("Sets the set on the target GameObject.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class SetState : TargetGameObjectAction
    {
        [Tooltip("The name of the state that should be set.")]
        public SharedVariable<string> m_StateName;
        [Tooltip("Should the state name be activated?")]
        public SharedVariable<bool> m_ActivateState = true;

        /// <summary>
        /// Tries to set the state.
        /// </summary>
        /// <returns>Success if the state was set.</returns>
        public override TaskStatus OnUpdate()
        {
            StateManager.SetState(gameObject, m_StateName.Value, m_ActivateState.Value);
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_StateName = string.Empty;
            m_ActivateState = true;
        }
    }
}
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
    using Opsive.UltimateCharacterController.Character;
    using UnityEngine;

    [NodeDescription("Sets the target of the LookSource.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer/integrations/opsive-character-controllers/")]
    public class SetAimTarget : TargetGameObjectAction
    {
        [Tooltip("The GameObject that the character should aim at.")]
        public SharedVariable<GameObject> m_AimTarget;

        private GameObject m_PrevTarget;
        private LocalLookSource m_LocalLookSource;

        /// <summary>
        /// Retrieves the local look source.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_LocalLookSource = gameObject.GetCachedComponent<LocalLookSource>();
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Tries to set the local look source.
        /// </summary>
        /// <returns>Success if the local look source was set.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_LocalLookSource == null) {
                return TaskStatus.Failure;
            }

            // The look source exists - set the target.
            if (m_AimTarget.Value != null) {
                m_LocalLookSource.Target = m_AimTarget.Value.transform;
            } else {
                m_LocalLookSource.Target = null;
            }
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_AimTarget = null;
        }
    }
}
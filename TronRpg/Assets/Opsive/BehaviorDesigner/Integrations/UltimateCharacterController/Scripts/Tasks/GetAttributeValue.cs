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

    [NodeDescription("Stores the attribute value.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class GetAttributeValue : TargetGameObjectAction
    {
        [Tooltip("The name of the attribute.")]
        public SharedVariable<string> m_AttributeName = "Health";
        [Tooltip("The location to store the value of.")]
        [RequireShared] public SharedVariable<float> m_StoreResult;

        private GameObject m_PrevTarget;
        private string m_PrevAttributeName;
        private AttributeManager m_AttributeManager;
        private Attribute m_Attribute;

        /// <summary>
        /// Retrieves the health component.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_AttributeManager = gameObject.GetCachedComponent<AttributeManager>();
                m_PrevTarget = gameObject;
            }

            if (m_AttributeManager != null && m_AttributeName.Value != m_PrevAttributeName) {
                m_Attribute = m_AttributeManager.GetAttribute(m_AttributeName.Value);
                m_PrevAttributeName = m_AttributeName.Value;
            }
        }

        /// <summary>
        /// Sets the value of the attribute with the specified name.
        /// </summary>
        /// <returns>Success if the value was successfully set.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_AttributeManager == null || m_Attribute == null) {
                return TaskStatus.Failure;
            }

            m_StoreResult.Value = m_Attribute.Value;
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_AttributeName = "Health";
            m_StoreResult = null;
        }
    }
}
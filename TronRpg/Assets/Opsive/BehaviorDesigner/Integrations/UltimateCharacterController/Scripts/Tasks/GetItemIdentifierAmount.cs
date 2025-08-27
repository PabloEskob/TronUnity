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
    using Opsive.UltimateCharacterController.Inventory;
    using UnityEngine;

    [NodeDescription("Gets the amount of the specified ItemIdentifier that the agent has.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class GetItemIdentifierAmount : TargetGameObjectAction
    {
        [Tooltip("The ItemType to get the count of.")]
        public SharedVariable<ItemType> m_ItemType;
        [Tooltip("The amount of the ItemType that the agent has.")]
        [RequireShared] public SharedVariable<int> m_StoreResult;

        private GameObject m_PrevTarget;
        private InventoryBase m_Inventory;

        /// <summary>
        /// Retrieves the inventory component.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_Inventory = gameObject.GetCachedComponent<InventoryBase>();
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Sets the value of the ItemType count.
        /// </summary>
        /// <returns>Success if the value was successfully set.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null || m_ItemType.Value == null) {
                return TaskStatus.Failure;
            }

            m_StoreResult.Value = m_Inventory.GetItemIdentifierAmount(m_ItemType.Value);
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_ItemType = null;
            m_StoreResult = null;
        }
    }
}
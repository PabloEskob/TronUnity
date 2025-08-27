/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateInventorySystem
{
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.UltimateInventorySystem.Equipping;
    using UnityEngine;

    [NodeDescription("Use an item object from the equipper.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class UseItemObject : TargetGameObjectAction
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedVariable<int> m_ItemObjectSlotIndex;
        [Tooltip("The amount to check within the inventory.")]
        public SharedVariable<int> m_ActionIndex;

        private UsableEquippedItemsHandler m_UsableEquippedItemsHandler;
        private GameObject m_PrevGameObject;

        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_UsableEquippedItemsHandler = gameObject.GetComponent<UsableEquippedItemsHandler>();
                m_PrevGameObject = gameObject;
            }
        }

        /// <summary>
        /// Returns success if the item was added correctly otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_UsableEquippedItemsHandler == null) { return TaskStatus.Failure; }
            
            m_UsableEquippedItemsHandler.UseItem(m_ItemObjectSlotIndex.Value,m_ActionIndex.Value);

            return TaskStatus.Success;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_ItemObjectSlotIndex = 0;
            m_ActionIndex = 0;
        }
    }
}
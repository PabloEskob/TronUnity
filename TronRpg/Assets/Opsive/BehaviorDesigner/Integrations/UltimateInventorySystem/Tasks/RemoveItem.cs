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
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using UnityEngine;

    [NodeDescription("Remove an amount of item from the inventory.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class RemoveItem : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedVariable<GameObject> m_TargetGameObject;
        [Tooltip("The amount to remove from the inventory.")]
        public SharedVariable<int> m_Amount;
        [Tooltip("The item to remove from the inventory.")]
        public SharedVariable<ItemDefinition> m_ItemDefinition;
        [Tooltip("The Item collection to look for. If none the check will be done on the entire inventory.")]
        public SharedVariable<ItemCollectionPurpose> m_ItemCollectionPurpose;

        private Inventory m_Inventory;
        private GameObject m_PrevGameObject;

        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_Inventory = gameObject.GetComponent<Inventory>();
                m_PrevGameObject = gameObject;
            }
        }

        /// <summary>
        /// Returns success if the item was removed correctly otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null) { return TaskStatus.Failure; }
            
            ItemInfo itemInfoRemoved;
            var itemCollection = m_Inventory.GetItemCollection(m_ItemCollectionPurpose.Value);
            if (itemCollection != null) {
                itemInfoRemoved = itemCollection.RemoveItem(m_ItemDefinition.Value, m_Amount.Value);
            } else {
                itemInfoRemoved = m_Inventory.RemoveItem(m_ItemDefinition.Value, m_Amount.Value);
            }
            
            return itemInfoRemoved.Amount != 0 ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_Amount = 1;
            m_ItemDefinition = null;
            m_ItemCollectionPurpose = ItemCollectionPurpose.None;
        }
    }
}
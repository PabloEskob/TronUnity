/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateInventorySystem
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using UnityEngine;

    [NodeDescription("Add an amount of item to the inventory.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class AddItem : TargetGameObjectAction
    {
        [Tooltip("The amount to check within the inventory.")]
        public SharedVariable<int> m_Amount;
        [Tooltip("The item to check within the inventory.")]
        public SharedVariable<ItemDefinition> m_ItemDefinition;
        [Tooltip("The Item collection to look for. If none the check will be done on the entire inventory.")]
        public SharedVariable<ItemCollectionPurpose> m_ItemCollectionPurpose;

        // Cache the inventory component
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
        /// Returns success if the item was added correctly otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null) { return TaskStatus.Failure; }

            var itemCollection = m_Inventory.GetItemCollection(m_ItemCollectionPurpose.Value);
            if (itemCollection != null) {
                var info = itemCollection.AddItem((ItemInfo) (m_Amount.Value, InventorySystemManager.CreateItem(m_ItemDefinition.Value)));
                return info.Amount == m_Amount.Value ? TaskStatus.Success : TaskStatus.Failure;
            }

            var itemInfo = m_Inventory.AddItem((ItemInfo) (m_Amount.Value, InventorySystemManager.CreateItem(m_ItemDefinition.Value)));
            return itemInfo.Amount == m_Amount.Value ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_TargetGameObject = null;
            m_Amount = 1;
            m_ItemDefinition = null;
            m_ItemCollectionPurpose = ItemCollectionPurpose.None;
        }
    }
}
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
    using Opsive.UltimateInventorySystem.DropsAndPickups;
    using UnityEngine;

    [NodeDescription("Drop an item.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class DropItemFromInventory : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedVariable<GameObject> m_TargetGameObject;
        [Tooltip("The Item Object Spawner is a component in the scene, use its ID to find it and spawn an Item Object.")]
        public SharedVariable<int> m_ItemObjectSpawnerID;
        [Tooltip("The amount to check within the inventory.")]
        public SharedVariable<int> m_Amount;
        [Tooltip("The item to check within the inventory.")]
        public SharedVariable<ItemDefinition> m_ItemDefinition;
        [Tooltip("Check inherently if the item is within the item definition.")]
        public SharedVariable<bool> m_CheckInherently;
        [Tooltip("The amount to check within the inventory.")]
        public SharedVariable<bool> m_RemoveItemOnDrop;
        [Tooltip("The Item collection to look for. If none the check will be done on the entire inventory.")]
        public SharedVariable<ItemCollectionPurpose> m_ItemCollectionPurpose;

        // Cache the inventory component
        private Inventory m_Inventory;
        private GameObject m_PrevGameObject;
        private ItemObjectSpawner m_ItemObjectSpawner;
        
        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_Inventory = gameObject.GetComponent<Inventory>();
                m_PrevGameObject = gameObject;
            }

            m_ItemObjectSpawner = InventorySystemManager.GetGlobal<ItemObjectSpawner>((uint)m_ItemObjectSpawnerID.Value);
        }

        /// <summary>
        /// Returns success if the item was added correctly otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null) { return TaskStatus.Failure; }

            ItemInfo? itemInfo = null;
            var itemCollection = m_Inventory.GetItemCollection(m_ItemCollectionPurpose.Value);
            if (itemCollection != null) {
                itemInfo = itemCollection.GetItemInfo(m_ItemDefinition.Value, m_CheckInherently.Value);
            } else {
                itemInfo = m_Inventory.GetItemInfo(m_ItemDefinition.Value, m_CheckInherently.Value);
            }

            if (!itemInfo.HasValue) { return TaskStatus.Failure; }
            itemInfo = (m_Amount.Value, itemInfo.Value);
            if (m_RemoveItemOnDrop.Value) {
                itemInfo = m_Inventory.RemoveItem(itemInfo.Value);
            }
            m_ItemObjectSpawner.Spawn(itemInfo.Value, m_Inventory.transform.position);
            
            return TaskStatus.Success;
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
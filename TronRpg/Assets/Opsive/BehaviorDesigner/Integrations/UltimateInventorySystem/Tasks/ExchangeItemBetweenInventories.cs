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

    [NodeDescription("Exchange Items Between Inventories.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class ExchangeItemBetweenInventories : TargetGameObjectAction
    {
        [Tooltip("The game object with the other inventory.")]
        public SharedVariable<GameObject> m_OtherInventoryGameObject;
        [Tooltip("The amount to check within the inventory.")]
        public SharedVariable<int> m_Amount;
        [Tooltip("The item to check within the inventory.")]
        public SharedVariable<ItemDefinition> m_ItemDefinition;
        [Tooltip("Check inherently if the item is within the item definition.")]
        public SharedVariable<bool> m_CheckInherently;
        [Tooltip("Give or retrieve the item.")]
        public SharedVariable<bool> m_GiveItem;

        private Inventory m_Inventory;
        private GameObject m_PrevGameObject;
        
        private Inventory m_OtherInventory;
        private GameObject m_PrevOtherGameObject;
        
        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_Inventory = gameObject.GetComponent<Inventory>();
                m_PrevGameObject = gameObject;
            }

            var currentOtherGameObject = m_OtherInventoryGameObject.Value;
            if (currentOtherGameObject != m_PrevOtherGameObject) {
                m_OtherInventory = currentOtherGameObject.GetComponent<Inventory>();
                m_PrevOtherGameObject = currentOtherGameObject;
            }
        }

        /// <summary>
        /// Returns success if the item was added correctly otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null) { return TaskStatus.Failure; }

            ItemInfo? itemInfo = null;

            var giver = m_GiveItem.Value ? m_Inventory : m_OtherInventory; 
            var receiver = m_GiveItem.Value ? m_OtherInventory : m_Inventory;
            itemInfo = giver.GetItemInfo(m_ItemDefinition.Value, m_CheckInherently.Value);
            if (!itemInfo.HasValue) { return TaskStatus.Failure; }

            var itemInfoValue = (ItemInfo)(Mathf.Min(m_Amount.Value, itemInfo.Value.Amount),itemInfo.Value);
            itemInfoValue = giver.RemoveItem(itemInfoValue);
            receiver.AddItem(itemInfoValue);
            
            return TaskStatus.Success;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_Amount = 1;
            m_ItemDefinition = null;
            m_OtherInventoryGameObject = null;
            m_GiveItem = false;
        }
    }
}
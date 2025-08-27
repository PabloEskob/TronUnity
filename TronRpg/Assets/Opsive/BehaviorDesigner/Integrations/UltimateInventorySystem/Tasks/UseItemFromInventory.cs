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
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.UI.DataContainers;
    using UnityEngine;

    [NodeDescription("Use an item action.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class UseItemFromInventory : TargetGameObjectAction
    {
        [Tooltip("The Item Action Set with the actions to use.")]
        public SharedVariable<CategoryItemActionSet> m_ItemActionSet;
        [Tooltip("The Item Action index to use.")]
        public SharedVariable<int> m_ActionIndex;
        [Tooltip("Use all the amount in the Collection?.")]
        public SharedVariable<bool> m_UseAllTheAmount;
        [Tooltip("The amount to check within the inventory (only if Use All the amount is false).")]
        public SharedVariable<int> m_Amount;
        [Tooltip("The item Definition to use.")]
        public SharedVariable<ItemDefinition> m_ItemDefinition;
        [Tooltip("Check inherently if the item is within the item definition.")]
        public SharedVariable<bool> m_CheckInherently;
        [Tooltip("The Item collection to look for. If none the check will be done on the entire inventory.")]
        public SharedVariable<string> m_ItemCollectionName;

        private Inventory m_Inventory;
        private ItemUser m_ItemUser;
        private GameObject m_PrevGameObject;

        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_Inventory = gameObject.GetComponent<Inventory>();
                m_ItemUser = gameObject.GetComponent<ItemUser>();
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

            ItemInfo? itemInfo = null;
            var itemCollection = m_Inventory.GetItemCollection(m_ItemCollectionName.Value);
            if (itemCollection != null) {
                itemInfo = itemCollection.GetItemInfo(m_ItemDefinition.Value, m_CheckInherently.Value);
            } else {
                itemInfo = m_Inventory.GetItemInfo(m_ItemDefinition.Value, m_CheckInherently.Value);
            }

            if (!itemInfo.HasValue) { return TaskStatus.Failure; }

            var itemInfoToUse = m_UseAllTheAmount.Value ? itemInfo.Value : (m_Amount.Value, itemInfo.Value);
            var success = m_ItemActionSet.Value.UseItemAction(itemInfoToUse,m_ItemUser,m_ActionIndex.Value);
            return success ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_ItemActionSet = null;
            m_ActionIndex = 0;
            m_ItemUser = null;
            m_Amount = 1;
            m_ItemDefinition = null;
            m_CheckInherently = false;
            m_ItemCollectionName = "MainCollection";
        }
    }
}
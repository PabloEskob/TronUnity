/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateInventorySystem
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using UnityEngine;

    [NodeDescription("Determines if an inventory has at least the amount of item specified.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class CompareItemAmount : TargetGameObjectConditional
    {
        [Tooltip("Choose how to compare the item amount.")]
        public SharedVariable<Compare> m_Compare;
        [Tooltip("The amount to check within the inventory.")]
        public SharedVariable<int> m_Amount;
        [Tooltip("The item to check within the inventory.")]
        public SharedVariable<ItemDefinition> m_ItemDefinition;
        [Tooltip("Check inherently if the item is equivalent.")]
        public SharedVariable<bool> m_CheckInherently;
        [Tooltip("Check inherently if the item is equivalent.")]
        public SharedVariable<bool> m_CountStacks;
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
        /// Returns success if the inventory has the item amount specified, otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null) { return TaskStatus.Failure; }

            var currentAmount = 0;
            var itemCollection = m_Inventory.GetItemCollection(m_ItemCollectionPurpose.Value);
            if (itemCollection != null) {
                currentAmount = itemCollection.GetItemAmount(m_ItemDefinition.Value, m_CheckInherently.Value, m_CountStacks.Value);
            } else {
                currentAmount = m_Inventory.GetItemAmount(m_ItemDefinition.Value, m_CheckInherently.Value, m_CountStacks.Value);
            }

            if(currentAmount == m_Amount.Value && (m_Compare.Value == Compare.SmallerOrEqualsTo || m_Compare.Value == Compare.GreaterOrEqualsTo || m_Compare.Value == Compare.EqualsTo)) {
                return TaskStatus.Success;
            }

            if (currentAmount >= m_Amount.Value && (m_Compare.Value == Compare.GreaterThan || m_Compare.Value == Compare.GreaterOrEqualsTo) ) {
                return TaskStatus.Success;
            }else if ( m_Compare.Value == Compare.SmallerThan || m_Compare.Value == Compare.SmallerOrEqualsTo) {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_Amount = 1;
            m_ItemDefinition = null;
            m_CheckInherently = false;
            m_CountStacks = false;
            m_ItemCollectionPurpose = ItemCollectionPurpose.None;
        }
    }
}
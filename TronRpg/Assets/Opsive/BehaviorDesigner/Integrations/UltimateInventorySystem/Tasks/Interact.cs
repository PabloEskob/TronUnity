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
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Interactions;
    using UnityEngine;

    [NodeDescription("Interact Inventory Interactor.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class Interact : TargetGameObjectAction
    {
        private Inventory m_Inventory;
        private InventoryInteractor m_Interactor;
        private GameObject m_PrevGameObject;
        
        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_Inventory = gameObject.GetComponent<Inventory>();
                m_PrevGameObject = gameObject;
                m_Interactor = gameObject.GetComponent<InventoryInteractor>();
            }
        }

        /// <summary>
        /// Returns success if the item was added correctly otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Inventory == null) { return TaskStatus.Failure; }

            m_Interactor.Interact();
            
            return TaskStatus.Success;
        }
    }
}
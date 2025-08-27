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
    using Opsive.UltimateInventorySystem.SaveSystem;
    using UnityEngine;

    [NodeDescription("Load the game using the Inventory save system.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class Load : Action
    {
        [Tooltip("The Save index specified which save file to load.")]
        public SharedVariable<int> m_SaveIndex;

        /// <summary>
        /// Returns success if the inventory has the item amount specified, otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            SaveSystemManager.Load(m_SaveIndex.Value);

            return TaskStatus.Success;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_SaveIndex = 0;
        }
    }
}
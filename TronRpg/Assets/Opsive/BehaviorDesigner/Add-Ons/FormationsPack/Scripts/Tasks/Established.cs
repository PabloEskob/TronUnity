/// ---------------------------------------------
/// Formations Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.FormationsPack.Runtime.Tasks
{
    using Opsive.BehaviorDesigner.AddOns.Shared.Runtime.Tasks;
    using Opsive.GraphDesigner.Runtime;
    using Unity.Entities;
    using UnityEngine;

    [Opsive.Shared.Utility.Description("Maintains the existing formation of agents based on their initial positions.")]
    [NodeIcon("5d39c46eb072a204390e7bac21b4dd1d", "b93da678e9a360a428095fb603c28bc8")]
    public class Established : FormationsTargetBase
    {
        private Vector3? m_CacheOffset;

        public override bool AssignOptimialIndicies => false;

        /// <summary>
        /// Calculate the position for this agent in the existing formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            if (m_CacheOffset.HasValue) {
                return center + m_CacheOffset.Value;
            }

            var offset = m_Transform.position - center;
            var position = center + offset;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            m_CacheOffset = position - center;

            return position;
        }

        /// <summary>
        /// The task has ended.
        /// </summary>
        public override void OnEnd()
        {
            base.OnEnd();
            m_CacheOffset = null;
        }

        /// <summary>
        /// Data structure for saving established formation state.
        /// </summary>
        private class EstablishedSaveData
        {
            [Tooltip("The base formation save data containing common formation state.")]
            public FormationSaveData BaseData;
            [Tooltip("Indicates if the agent has a valid cache offset.")]
            public bool HasCacheOffset;
            [Tooltip("The cached offset from the formation center for this agent's position.")]
            public Vector3 CacheOffset;
        }

        /// <summary>
        /// Returns the current task state.
        /// </summary>
        /// <param name="world">The DOTS world.</param>
        /// <param name="entity">The DOTS entity.</param>
        /// <returns>The current task state.</returns>
        public override object Save(World world, Entity entity)
        {
            var saveData = base.Save(world, entity);
            if (saveData == null) {
                return null;
            }

            var establishedSaveData = new EstablishedSaveData();
            establishedSaveData.BaseData = (FormationSaveData)saveData;
            if (m_CacheOffset.HasValue) {
                establishedSaveData.HasCacheOffset = true;
                establishedSaveData.CacheOffset = m_CacheOffset.Value;
            }
            return establishedSaveData;
        }

        /// <summary>
        /// Loads the previous task state.
        /// </summary>
        /// <param name="saveData">The previous task state.</param>
        /// <param name="world">The DOTS world.</param>
        /// <param name="entity">The DOTS entity.</param>
        public override void Load(object saveData, World world, Entity entity)
        {
            if (saveData == null) {
                return;
            }

            var establishedSaveData = (EstablishedSaveData)saveData;
            base.Load(establishedSaveData.BaseData, world, entity);
            if (establishedSaveData.HasCacheOffset) {
                m_CacheOffset = establishedSaveData.CacheOffset;
            }
        }
    }
} 
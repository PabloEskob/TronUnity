/// ---------------------------------------------
/// Formations Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.FormationsPack.Runtime.Tasks
{
    using Opsive.BehaviorDesigner.AddOns.Shared.Runtime.Tasks;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.Shared.Utility;
    using Unity.Entities;
    using UnityEngine;

    [Opsive.Shared.Utility.Description("Moves agents in a randomly spread out line.")]
    [NodeIcon("193241218c6236140950040b2c188292", "d21e14821eb363d4a8ceb2575db2500e")]
    public class Skirmisher : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the skirmisher line should be oriented.
        /// </summary>
        public enum SkirmisherOrientation
        {
            Forward,    // The line extends forward from the leader.
            Backward,   // The line extends backward from the leader.
            Left,       // The line extends to the left of the leader.
            Right       // The line extends to the right of the leader.
        }

        [Tooltip("The minimum and maximum spacing between agents along the skirmisher line.")]
        [SerializeField] protected SharedVariable<MinMaxFloat> m_SkirmisherSpacing = new MinMaxFloat(2, 4);
        [Tooltip("The minimum and maximum spacing between agents perpendicular to the skirmisher line.")]
        [SerializeField] protected SharedVariable<MinMaxFloat> m_PerpendicularSpacing = new MinMaxFloat(2, 2);
        [Tooltip("Specifies how the skirmisher line should be oriented.")]
        [SerializeField] protected SharedVariable<SkirmisherOrientation> m_SkirmisherOrientation = SkirmisherOrientation.Forward;

        private Vector3? m_CachedPosition;

        public override bool AssignOptimialIndicies => false;

        /// <summary>
        /// Calculate the position for this agent in the skirmisher formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            if (index == 0) {
                return center;
            }

            // Return cached position if it exists.
            if (m_CachedPosition.HasValue) {
                return center + m_CachedPosition.Value;
            }

            // Determine the base direction based on orientation.
            Vector3 direction;
            switch (m_SkirmisherOrientation.Value) {
                case SkirmisherOrientation.Backward:
                    direction = -forward;
                    break;
                case SkirmisherOrientation.Left:
                    if (m_Is2D) {
                        direction = new Vector3(-forward.y, forward.x, 0);
                    } else {
                        direction = Vector3.Cross(forward, m_Transform.up);
                    }
                    break;
                case SkirmisherOrientation.Right:
                    if (m_Is2D) {
                        direction = new Vector3(forward.y, -forward.x, 0);
                    } else {
                        direction = -Vector3.Cross(forward, m_Transform.up);
                    }
                    break;
                default: // Forward.
                    direction = forward;
                    break;
            }

            // Calculate the agent's position.
            var skirmisherOffset = index * m_SkirmisherSpacing.Value.RandomValue;
            var perpendicularOffset = m_PerpendicularSpacing.Value.RandomValue;
            var perpendicular = m_Is2D ? new Vector3(-direction.y, direction.x, 0) : Vector3.Cross(direction, m_Transform.up);
            var position = center + direction.normalized * skirmisherOffset + perpendicular.normalized * perpendicularOffset;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            // Cache the local position to allow for multiple calls to CalculateFormationPosition.
            m_CachedPosition = position - center;

            return position;
        }

        /// <summary>
        /// The task has ended.
        /// </summary>
        public override void OnEnd()
        {
            base.OnEnd();
            m_CachedPosition = null;
        }

        /// <summary>
        /// Data structure for saving skirmisher formation state.
        /// </summary>
        private class SkirmisherSaveData
        {
            [Tooltip("The base formation save data containing common formation state.")]
            public FormationSaveData BaseData;
            [Tooltip("Indicates if the agent has a valid cached position.")]
            public bool HasCachedPosition;
            [Tooltip("The cached position offset from the formation center for this agent.")]
            public Vector3 CachedPosition;
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

            var skirmisherSaveData = new SkirmisherSaveData();
            skirmisherSaveData.BaseData = (FormationSaveData)saveData;
            if (m_CachedPosition.HasValue) {
                skirmisherSaveData.HasCachedPosition = true;
                skirmisherSaveData.CachedPosition = m_CachedPosition.Value;
            }
            return skirmisherSaveData;
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

            var skirmisherSaveData = (SkirmisherSaveData)saveData;
            base.Load(skirmisherSaveData.BaseData, world, entity);
            if (skirmisherSaveData.HasCachedPosition) {
                m_CachedPosition = skirmisherSaveData.CachedPosition;
            }
        }

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_SkirmisherSpacing = new MinMaxFloat(2, 4);
            m_PerpendicularSpacing = new MinMaxFloat(-2, 2);
            m_SkirmisherOrientation = SkirmisherOrientation.Forward;
        }
    }
} 
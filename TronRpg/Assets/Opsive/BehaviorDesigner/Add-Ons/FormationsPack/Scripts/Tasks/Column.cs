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
    using UnityEngine;

    [Opsive.Shared.Utility.Description("Moves agents in a column formation.")]
    [NodeIcon("573fbefdf295a1d44b047c5d2ddeb7f6", "eb677b0ea8be0474dbf16509c7c8ce3c")]
    public class Column : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how each column should be aligned.
        /// </summary>
        public enum ColumnAlignment
        {
            Front,  // Align the agents in front of the leader.
            Center, // Align the agents with the leader in the center.
            Back    // Align the agents with the leader in the back.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<Vector2> m_Spacing = new Vector2(2f, 2f);
        [Tooltip("The maximum number of agents allowed in a single column.")]
        [SerializeField] protected SharedVariable<int> m_MaxAgentsPerColumn = 5;
        [Tooltip("Specifies how each column should be aligned.")]
        [SerializeField] protected SharedVariable<ColumnAlignment> m_ColumnAlignment = ColumnAlignment.Front;

        /// <summary>
        /// Calculate the position for this agent in the column formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            var column = index / m_MaxAgentsPerColumn.Value;
            var row = index % m_MaxAgentsPerColumn.Value;
            
            // Calculate the offset from center based on column and row.
            float verticalOffset;
            switch (m_ColumnAlignment.Value)
            {
                case ColumnAlignment.Front:
                    verticalOffset = -row * m_Spacing.Value.y;
                    break;
                case ColumnAlignment.Back:
                    verticalOffset = row * m_Spacing.Value.y;
                    break;
                default: // Center
                    verticalOffset = (row - (m_MaxAgentsPerColumn.Value - 1) / 2f) * m_Spacing.Value.y;
                    break;
            }

            // Center the columns horizontally by offsetting from the center.
            var totalColumns = Mathf.CeilToInt((float)totalAgents / m_MaxAgentsPerColumn.Value);
            var horizontalOffset = (column - (totalColumns - 1) / 2f) * m_Spacing.Value.x;

            // The position and rotation depends on the perspective.
            Vector3 localPosition;
            Quaternion rotation;
            if (m_Is2D) {
                localPosition = new Vector3(horizontalOffset, verticalOffset, 0);

                var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                localPosition = new Vector3(horizontalOffset, 0, verticalOffset);

                var direction = Vector3.ProjectOnPlane(forward, m_Transform.up).normalized;
                var right = Vector3.Cross(direction, m_Transform.up).normalized;
                rotation = Quaternion.LookRotation(direction, m_Transform.up);
            }

            // Calculate the agent's position.
            var position = center + rotation * localPosition;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            return position;
        }

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_Spacing = new Vector2(2f, 2f);
            m_MaxAgentsPerColumn = 5;
            m_ColumnAlignment = ColumnAlignment.Front;
        }
    }
} 
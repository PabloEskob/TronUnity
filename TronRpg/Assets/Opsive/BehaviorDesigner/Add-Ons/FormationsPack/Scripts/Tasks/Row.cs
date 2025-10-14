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

    [Opsive.Shared.Utility.Description("Moves agents in a row formation.")]
    [NodeIcon("974487773b745014fa4734d6da6afcd0", "09d88c145bcb8f54b936f4bd7966421e")]
    public class Row : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how each row should be aligned.
        /// </summary>
        public enum RowAlignment
        {
            Left,   // Align the agents to the left of the leader.
            Center, // Align the agents with the leader in the center.
            Right   // Align the agents to the right of the leader.
        }

        [Tooltip("The maximum number of agents allowed in a single row.")]
        [SerializeField] protected SharedVariable<int> m_MaxAgentsPerRow = 5;
        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<Vector2> m_Spacing = new Vector2(2f, 2f);
        [Tooltip("Specifies how each row should be aligned.")]
        [SerializeField] protected SharedVariable<RowAlignment> m_RowAlignment = RowAlignment.Center;

        /// <summary>
        /// Calculate the position for this agent in the row formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            var row = index / m_MaxAgentsPerRow.Value;
            var column = index % m_MaxAgentsPerRow.Value;
            
            // Calculate the offset from center based on row and column
            float horizontalOffset;
            switch (m_RowAlignment.Value)
            {
                case RowAlignment.Left:
                    horizontalOffset = column * m_Spacing.Value.x;
                    break;
                case RowAlignment.Right:
                    horizontalOffset = -(m_MaxAgentsPerRow.Value - 1 - column) * m_Spacing.Value.x;
                    break;
                default: // Center.
                    if (index == 0) { // The leader agent should not have any offset.
                        horizontalOffset = 0; 
                    } else {
                        // Even indices go left, odd indices go right.
                        var sideOffset = Mathf.CeilToInt(column / 2f);
                        horizontalOffset = (column % 2 == 1) ? sideOffset * m_Spacing.Value.x : -sideOffset * m_Spacing.Value.x;
                    }
                    break;
            }
            
            // Center the rows vertically by offsetting from the center.
            var totalRows = Mathf.CeilToInt((float)totalAgents / m_MaxAgentsPerRow.Value);
            var verticalOffset = (row - (totalRows - 1) / 2f) * m_Spacing.Value.y;

            // The position and rotation depends on the perspective.
            Vector3 localPosition;
            Quaternion rotation;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(horizontalOffset, verticalOffset, 0);
                var forward2D = new Vector2(forward.x, forward.y).normalized;
                rotation = Quaternion.Euler(0, 0, Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg);
            } else {
                // Use the XZ plane for 3D.
                localPosition = new Vector3(horizontalOffset, 0, verticalOffset);
                rotation = Quaternion.LookRotation(forward, m_Transform.up);
            }

            // Calculate the agent's position.
            var position = center + rotation * localPosition;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            return position;
        }
    }
}

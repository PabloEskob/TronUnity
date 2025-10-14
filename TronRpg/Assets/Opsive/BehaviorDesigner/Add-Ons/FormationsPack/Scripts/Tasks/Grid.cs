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

    [Opsive.Shared.Utility.Description("Moves agents in a grid formation.")]
    [NodeIcon("96c299aee0b43f84a92985c11b32347c", "c7c0d1dd6c514e44bbed24d10b5f2485")]
    public class Grid : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the grid should be filled.
        /// </summary>
        public enum GridFillMode
        {
            RowFirst,    // Fill the grid row by row.
            ColumnFirst  // Fill the grid column by column.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<Vector2> m_Spacing = new Vector2(2f, 2f);
        [Tooltip("The number of columns in the grid.")]
        [SerializeField] protected SharedVariable<int> m_Columns = 3;
        [Tooltip("Specifies how the grid should be filled.")]
        [SerializeField] protected SharedVariable<GridFillMode> m_FillMode = GridFillMode.RowFirst;

        /// <summary>
        /// Calculate the position for this agent in the grid formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            int row, column;
            if (m_FillMode.Value == GridFillMode.RowFirst) {
                row = index / m_Columns.Value;
                column = index % m_Columns.Value;
            } else { // ColumnFirst.
                column = index / m_Columns.Value;
                row = index % m_Columns.Value;
            }

            // Calculate the offset from center.
            var horizontalOffset = (column - (m_Columns.Value - 1) / 2f) * m_Spacing.Value.x;
            var totalRows = Mathf.CeilToInt((float)totalAgents / m_Columns.Value);
            var verticalOffset = ((totalRows - 1 - row) - (totalRows - 1) / 2f) * m_Spacing.Value.y;

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
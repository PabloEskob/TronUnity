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

    [Opsive.Shared.Utility.Description("Moves agents in a square formation.")]
    [NodeIcon("a2b6f6e80085e764797073a3a557ce6b", "a34dde275b4fe784a92955a3181b430c")]
    public class Square : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the square should be filled.
        /// </summary>
        public enum SquareFillMode
        {
            RowFirst,   // Fill the square row by row.
            ColumnFirst // Fill the square column by column.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<float> m_Spacing = 2f;
        [Tooltip("Specifies how the square should be filled.")]
        [SerializeField] protected SharedVariable<SquareFillMode> m_FillMode = SquareFillMode.RowFirst;

        /// <summary>
        /// Calculate the position for this agent in the square formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            // Calculate the size of the square.
            var columns = Mathf.CeilToInt(Mathf.Sqrt(totalAgents));
            var rows = Mathf.CeilToInt((float)totalAgents / columns);
            int row, column;
            switch (m_FillMode.Value) {
                case SquareFillMode.RowFirst:
                    row = index / columns;
                    column = index % columns;
                    break;
                default: // ColumnFirst.
                    column = index / rows;
                    row = index % rows;
                    break;
            }

            // Calculate the offset from center.
            var horizontalOffset = (column - (columns - 1) / 2f) * m_Spacing.Value;
            var verticalOffset = ((columns - 1 - row) - (columns - 1) / 2f) * m_Spacing.Value;

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
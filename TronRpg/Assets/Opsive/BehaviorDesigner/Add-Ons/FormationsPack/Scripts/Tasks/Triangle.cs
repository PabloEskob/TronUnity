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

    [Opsive.Shared.Utility.Description("Moves agents in a triangular formation.")]
    [NodeIcon("1678a9d2ab34b7b4e937d6f8c4211e3d", "f5eda0d3aeb370442ba4fba552d0163d")]
    public class Triangle : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the triangle should be oriented.
        /// </summary>
        public enum TriangleOrientation
        {
            Forward,    // The triangle points forward.
            Backward,   // The triangle points backward.
            Left,       // The triangle points left.
            Right       // The triangle points right.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<float> m_Spacing = 2f;
        [Tooltip("Specifies how the triangle should be oriented.")]
        [SerializeField] protected SharedVariable<TriangleOrientation> m_TriangleOrientation = TriangleOrientation.Forward;

        /// <summary>
        /// Calculate the position for this agent in the triangle formation.
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

            // Calculate the position in local space.
            var row = Mathf.FloorToInt((Mathf.Sqrt(8 * index + 1) - 1) / 2);
            var rowStart = row * (row + 1) / 2;
            var positionInRow = index - rowStart;
            var horizontalOffset = (positionInRow - row / 2f) * m_Spacing.Value;
            var verticalOffset = -row * m_Spacing.Value;

            // Determine the position and rotation based on orientation.
            Vector3 localPosition;
            Quaternion rotation;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(horizontalOffset, verticalOffset, 0);

                var forward2D = new Vector2(forward.x, forward.y).normalized;
                var angle = Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg;
                switch (m_TriangleOrientation.Value) {
                    case TriangleOrientation.Backward:
                        angle += 180f;
                        break;
                    case TriangleOrientation.Left:
                        angle += 90f;
                        break;
                    case TriangleOrientation.Right:
                        angle -= 90f;
                        break;
                }
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                // Use the XZ plane in 3D.
                localPosition = new Vector3(horizontalOffset, 0, verticalOffset);
                switch (m_TriangleOrientation.Value) {
                    case TriangleOrientation.Backward:
                        rotation = Quaternion.LookRotation(-forward, m_Transform.up);
                        break;
                    case TriangleOrientation.Left:
                        rotation = Quaternion.LookRotation(Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    case TriangleOrientation.Right:
                        rotation = Quaternion.LookRotation(-Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    default: // Forward.
                        rotation = Quaternion.LookRotation(forward, m_Transform.up);
                        break;
                }
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
            m_Spacing = 2f;
            m_TriangleOrientation = TriangleOrientation.Forward;
        }
    }
} 
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

    [Opsive.Shared.Utility.Description("Moves agents in a line formation.")]
    [NodeIcon("72475ac510087954e96069217d69d60c", "ea82759ce723a5f4dad225dcee24f984")]
    public class Line : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the line should be oriented.
        /// </summary>
        public enum LineOrientation
        {
            Forward,    // The line extends forward from the leader.
            Backward,   // The line extends backward from the leader.
            Left,       // The line extends to the left of the leader.
            Right       // The line extends to the right of the leader.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<float> m_Spacing = 2f;
        [Tooltip("Specifies how the line should be oriented.")]
        [SerializeField] protected SharedVariable<LineOrientation> m_LineOrientation = LineOrientation.Forward;

        /// <summary>
        /// Calculate the position for this agent in the line formation.
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
            var offset = index * m_Spacing.Value;
            Vector3 localPosition;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                switch (m_LineOrientation.Value) {
                    case LineOrientation.Backward:
                        localPosition = new Vector3(-offset, 0, 0);
                        break;
                    case LineOrientation.Left:
                        localPosition = new Vector3(0, offset, 0);
                        break;
                    case LineOrientation.Right:
                        localPosition = new Vector3(0, -offset, 0);
                        break;
                    default: // Forward.
                        localPosition = new Vector3(offset, 0, 0);
                        break;
                }
            } else {
                // Use the XZ plane for 3D.
                switch (m_LineOrientation.Value) {
                    case LineOrientation.Backward:
                        localPosition = new Vector3(-offset, 0, 0);
                        break;
                    case LineOrientation.Left:
                        localPosition = new Vector3(0, 0, offset);
                        break;
                    case LineOrientation.Right:
                        localPosition = new Vector3(0, 0, -offset);
                        break;
                    default: // Forward.
                        localPosition = new Vector3(offset, 0, 0);
                        break;
                }
            }

            // Determine the rotation based on orientation.
            Quaternion rotation;
            if (m_Is2D) {
                var forward2D = new Vector2(forward.x, forward.y).normalized;
                var angle = Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg;
                switch (m_LineOrientation.Value) {
                    case LineOrientation.Backward:
                        angle += 180f;
                        break;
                    case LineOrientation.Left:
                        angle += 90f;
                        break;
                    case LineOrientation.Right:
                        angle -= 90f;
                        break;
                }
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                switch (m_LineOrientation.Value) {
                    case LineOrientation.Backward:
                        rotation = Quaternion.LookRotation(-forward, m_Transform.up);
                        break;
                    case LineOrientation.Left:
                        rotation = Quaternion.LookRotation(Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    case LineOrientation.Right:
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
            m_LineOrientation = LineOrientation.Forward;
        }
    }
} 
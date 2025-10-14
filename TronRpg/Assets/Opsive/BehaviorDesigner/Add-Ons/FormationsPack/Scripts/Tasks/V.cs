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

    [Opsive.Shared.Utility.Description("Moves agents in a V formation.")]
    [NodeIcon("99d1fda25aa6a444392ebb47208ed8d1", "6d1ddf315550d4a47a0e6c5a9b59f316")]
    public class V : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the V should be oriented.
        /// </summary>
        public enum VOrientation
        {
            Forward,    // The V points forward.
            Backward,   // The V points backward.
            Left,       // The V points left.
            Right       // The V points right.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<float> m_Spacing = 2f;
        [Tooltip("The angle between the two lines of the V in degrees.")]
        [SerializeField] protected SharedVariable<float> m_VAngle = 60f;
        [Tooltip("Specifies how the V should be oriented.")]
        [SerializeField] protected SharedVariable<VOrientation> m_VOrientation = VOrientation.Forward;

        /// <summary>
        /// Calculate the position for this agent in the V formation.
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
            var line = (index - 1) % 2;
            var angle = (line == 0 ? -m_VAngle.Value : m_VAngle.Value) * Mathf.Deg2Rad;
            var distance = ((index - 1) / 2 + 1) * m_Spacing.Value;
            Vector3 localPosition;
            if (m_Is2D) {
            } else {
            }

            // Determine the position and rotation based on orientation.
            Quaternion rotation;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(Mathf.Sin(angle) * distance, -Mathf.Cos(angle) * distance, 0);

                var forward2D = new Vector2(forward.x, forward.y).normalized;
                angle = Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg;
                switch (m_VOrientation.Value) {
                    case VOrientation.Backward:
                        angle += 180f;
                        break;
                    case VOrientation.Left:
                        angle += 90f;
                        break;
                    case VOrientation.Right:
                        angle -= 90f;
                        break;
                }
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                // Use the XZ plane for 3D.
                localPosition = new Vector3(Mathf.Sin(angle) * distance, 0, -Mathf.Cos(angle) * distance);
                switch (m_VOrientation.Value) {
                    case VOrientation.Backward:
                        rotation = Quaternion.LookRotation(-forward, m_Transform.up);
                        break;
                    case VOrientation.Left:
                        rotation = Quaternion.LookRotation(Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    case VOrientation.Right:
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
            m_VAngle = 60f;
            m_VOrientation = VOrientation.Forward;
        }
    }
} 
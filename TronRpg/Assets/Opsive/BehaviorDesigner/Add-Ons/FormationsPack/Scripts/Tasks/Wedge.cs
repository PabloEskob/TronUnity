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

    [Opsive.Shared.Utility.Description("Moves agents in a wedge formation.")]
    [NodeIcon("cf869f0beeb479743abff62e9e7df277", "bc4b27d53369399408592d9405942821")]
    public class Wedge : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the wedge should be oriented.
        /// </summary>
        public enum WedgeOrientation
        {
            Forward,    // The wedge points forward.
            Backward,   // The wedge points backward.
            Left,       // The wedge points left.
            Right       // The wedge points right.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<float> m_Spacing = 2f;
        [Tooltip("The angle between the two sides of the wedge in degrees.")]
        [SerializeField] protected SharedVariable<float> m_WedgeAngle = 45f;
        [Tooltip("Specifies how the wedge should be oriented.")]
        [SerializeField] protected SharedVariable<WedgeOrientation> m_WedgeOrientation = WedgeOrientation.Forward;

        /// <summary>
        /// Calculate the position for this agent in the wedge formation.
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
            var angle = ((index - rowStart) - row / 2f) / (row / 2f) * (m_WedgeAngle.Value / 2f) * Mathf.Deg2Rad;
            var distance = (row + 1) * m_Spacing.Value;

            // Determine the position and rotation based on orientation.
            Quaternion rotation;
            Vector3 localPosition;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(Mathf.Sin(angle) * distance, -(row * m_Spacing.Value), 0);

                var forward2D = new Vector2(forward.x, forward.y).normalized;
                angle = Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg;
                switch (m_WedgeOrientation.Value) {
                    case WedgeOrientation.Backward:
                        angle += 180f;
                        break;
                    case WedgeOrientation.Left:
                        angle += 90f;
                        break;
                    case WedgeOrientation.Right:
                        angle -= 90f;
                        break;
                }
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                // Use the XZ plane for 3D.
                localPosition = new Vector3(Mathf.Sin(angle) * distance, 0, -(row * m_Spacing.Value));

                switch (m_WedgeOrientation.Value) {
                    case WedgeOrientation.Backward:
                        rotation = Quaternion.LookRotation(-forward, m_Transform.up);
                        break;
                    case WedgeOrientation.Left:
                        rotation = Quaternion.LookRotation(Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    case WedgeOrientation.Right:
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
            m_WedgeAngle = 45f;
            m_WedgeOrientation = WedgeOrientation.Forward;
        }
    }
} 
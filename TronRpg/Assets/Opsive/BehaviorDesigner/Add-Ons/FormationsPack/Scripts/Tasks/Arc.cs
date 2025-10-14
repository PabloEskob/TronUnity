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

    [Opsive.Shared.Utility.Description("Moves agents in an arc formation.")]
    [NodeIcon("99d1fda25aa6a444392ebb47208ed8d1", "6d1ddf315550d4a47a0e6c5a9b59f316")]
    public class Arc : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the arc should be oriented.
        /// </summary>
        public enum ArcOrientation
        {
            Forward,    // The arc faces forward with the leader at the center.
            Backward,   // The arc faces backward with the leader at the center.
            Left,       // The arc faces left with the leader at the center.
            Right       // The arc faces right with the leader at the center.
        }

        [Tooltip("The radius of the arc.")]
        [SerializeField] protected SharedVariable<float> m_Radius = 5f;
        [Tooltip("The angle of the arc in degrees.")]
        [SerializeField] protected SharedVariable<float> m_ArcAngle = 180f;
        [Tooltip("Specifies how the arc should be oriented.")]
        [SerializeField] protected SharedVariable<ArcOrientation> m_ArcOrientation = ArcOrientation.Forward;

        /// <summary>
        /// Calculate the position for this agent in the arc formation.
        /// </summary>
        /// <param name="index">The index of this agent in the formation.</param>
        /// <param name="totalAgents">The total number of agents in the formation.</param>
        /// <param name="center">The center position of the formation.</param>
        /// <param name="forward">The forward direction of the formation.</param>
        /// <param name="samplePosition">Should the position be sampled?</param>
        /// <returns>The position for this agent.</returns>
        public override Vector3 CalculateFormationPosition(int index, int totalAgents, Vector3 center, Vector3 forward, bool samplePosition)
        {
            if (totalAgents == 1) {
                return center;
            }

            // Calculate the position in local space
            var angleStep = m_ArcAngle.Value / (totalAgents - 1);
            var angle = -m_ArcAngle.Value / 2f + angleStep * index;
            var angleRad = angle * Mathf.Deg2Rad;

            // Determine the position and rotation based on orientation.
            Vector3 localPosition;
            Quaternion rotation;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(Mathf.Sin(angleRad) * m_Radius.Value, Mathf.Cos(angleRad) * m_Radius.Value, 0);

                var forward2D = new Vector2(forward.x, forward.y).normalized;
                angle = Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg;
                switch (m_ArcOrientation.Value) {
                    case ArcOrientation.Backward:
                        angle += 180f;
                        break;
                    case ArcOrientation.Left:
                        angle += 90f;
                        break;
                    case ArcOrientation.Right:
                        angle -= 90f;
                        break;
                }
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                // Use the XZ plane for 3D.
                localPosition = new Vector3(Mathf.Sin(angleRad) * m_Radius.Value, 0, Mathf.Cos(angleRad) * m_Radius.Value);

                switch (m_ArcOrientation.Value) {
                    case ArcOrientation.Backward:
                        rotation = Quaternion.LookRotation(-forward, m_Transform.up);
                        break;
                    case ArcOrientation.Left:
                        rotation = Quaternion.LookRotation(Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    case ArcOrientation.Right:
                        rotation = Quaternion.LookRotation(-Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    default: // Forward.
                        rotation = Quaternion.LookRotation(forward, m_Transform.up);
                        break;
                }
            }

            // Transform the local position to world space.
            var position = center + rotation * localPosition;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            return position;
        }
    }
} 
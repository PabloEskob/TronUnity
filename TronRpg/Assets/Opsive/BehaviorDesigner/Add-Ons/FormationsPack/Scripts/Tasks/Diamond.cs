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

    [Opsive.Shared.Utility.Description("Moves agents in a tactical diamond formation. Any additional agents will be placed at the back.")]
    [NodeIcon("7a7b13793325af04daf40a99ce91f078", "7dfece580043528489b6aa5a824fb777")]
    public class Diamond : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the diamond should be oriented.
        /// </summary>
        public enum DiamondOrientation
        {
            Forward,    // The diamond points forward.
            Backward,   // The diamond points backward.
            Left,       // The diamond points left.
            Right       // The diamond points right.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<float> m_Spacing = 2f;
        [Tooltip("The angle of the diamond's corners in degrees.")]
        [SerializeField] protected SharedVariable<float> m_DiamondAngle = 60f;
        [Tooltip("Specifies how the diamond should be oriented.")]
        [SerializeField] protected SharedVariable<DiamondOrientation> m_DiamondOrientation = DiamondOrientation.Forward;
        [Tooltip("The number of columns for additional agents at the back.")]
        [SerializeField] protected SharedVariable<int> m_ColumnCount = 1;

        /// <summary>
        /// Calculate the position for this agent in the diamond formation.
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
                return center; // Leader stays in center.
            }

            // Calculate the position in local space.
            Vector3 localPosition;
            var spacing = m_Spacing.Value;
            switch (index) { // Point man - directly in front
                case 1:
                    if (m_Is2D) {
                        localPosition = new Vector3(0, -spacing, 0);
                    } else {
                        localPosition = new Vector3(0, 0, -spacing);
                    }
                    break;
                case 2: // Left flanker.
                    if (m_Is2D) {
                        localPosition = new Vector3(-spacing, 0, 0);
                    } else {
                        localPosition = new Vector3(-spacing, 0, 0);
                    }
                    break;
                case 3: // Right flanker
                    if (m_Is2D) {
                        localPosition = new Vector3(spacing, 0, 0);
                    } else {
                        localPosition = new Vector3(spacing, 0, 0);
                    }
                    break;
                case 4: // Rear guard.
                    if (m_Is2D) {
                        localPosition = new Vector3(0, spacing, 0);
                    } else {
                        localPosition = new Vector3(0, 0, spacing);
                    }
                    break;
                default:
                    // For additional agents, place them in columns behind the rear guard.
                    var agentsInColumns = index - 4;
                    var columnCount = Mathf.Max(1, m_ColumnCount.Value); // Ensure at least 1 column.
                    var row = (agentsInColumns - 1) / columnCount;
                    var column = (agentsInColumns - 1) % columnCount;
                    
                    // Calculate the x offset based on column position.
                    var xOffset = 0f;
                    if (columnCount > 1) {
                        var totalWidth = spacing * (columnCount - 1);
                        xOffset = (column * spacing) - (totalWidth / 2f);
                    }
                    
                    if (m_Is2D) {
                        localPosition = new Vector3(xOffset, spacing * (row + 2), 0); // Add 2 to account for rear guard.
                    } else {
                        localPosition = new Vector3(xOffset, 0, spacing * (row + 2)); // Add 2 to account for rear guard.
                    }
                    break;
            }

            // Determine the rotation based on orientation.
            Quaternion rotation;
            if (m_Is2D) {
                var forward2D = new Vector2(forward.x, forward.y).normalized;
                var angle = Mathf.Atan2(forward2D.y, forward2D.x) * Mathf.Rad2Deg;
                switch (m_DiamondOrientation.Value) {
                    case DiamondOrientation.Backward:
                        angle += 180f;
                        break;
                    case DiamondOrientation.Left:
                        angle += 90f;
                        break;
                    case DiamondOrientation.Right:
                        angle -= 90f;
                        break;
                }
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                switch (m_DiamondOrientation.Value) {
                    case DiamondOrientation.Backward:
                        rotation = Quaternion.LookRotation(forward, m_Transform.up);
                        break;
                    case DiamondOrientation.Left:
                        rotation = Quaternion.LookRotation(-Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    case DiamondOrientation.Right:
                        rotation = Quaternion.LookRotation(Vector3.Cross(forward, m_Transform.up), m_Transform.up);
                        break;
                    default: // Forward.
                        rotation = Quaternion.LookRotation(-forward, m_Transform.up);
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

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_Spacing = 2f;
            m_DiamondAngle = 60f;
            m_DiamondOrientation = DiamondOrientation.Forward;
            m_ColumnCount = 1;
        }
    }
} 
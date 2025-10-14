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

    [Opsive.Shared.Utility.Description("Moves agents in an echelon formation.")]
    [NodeIcon("5f3a19b59ffa30646ba72d0988cc4a51", "9c07330b93008c34a9ce4dd050403fd5")]
    public class Echelon : FormationsTargetBase
    {
        /// <summary>
        /// Specifies the direction of the echelon.
        /// </summary>
        public enum EchelonDirection
        {
            Left,   // The echelon extends to the left.
            Right   // The echelon extends to the right.
        }

        [Tooltip("The spacing between agents.")]
        [SerializeField] protected SharedVariable<Vector2> m_Spacing = new Vector2(2f, 2f);
        [Tooltip("Specifies the direction of the echelon.")]
        [SerializeField] protected SharedVariable<EchelonDirection> m_EchelonDirection = EchelonDirection.Right;

        /// <summary>
        /// Calculate the position for this agent in the echelon formation.
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

            // Calculate the offset based on the echelon direction.
            var horizontalOffset = index * m_Spacing.Value.x * (m_EchelonDirection.Value == EchelonDirection.Right ? -1 : 1);
            var verticalOffset = index * m_Spacing.Value.y;

            // Determine the position and rotation based on orientation.
            Vector3 localPosition;
            Quaternion rotation;
            if (m_Is2D) {
                localPosition = new Vector3(horizontalOffset, verticalOffset, 0);

                var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(0, 0, angle);
            } else {
                localPosition = new Vector3(horizontalOffset, 0, verticalOffset);

                var direction = Vector3.ProjectOnPlane(forward, m_Transform.up).normalized;
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
            m_EchelonDirection = EchelonDirection.Right;
        }
    }
} 
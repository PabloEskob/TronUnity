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

    [Opsive.Shared.Utility.Description("Moves agents in a circular formation.")]
    [NodeIcon("3b70cafb9dc8d824384df1dc73fac81b", "d0e71951e5f078c4eb181641dd8e447d")]
    public class Circle : FormationsTargetBase
    {
        [Tooltip("The radius of the circle.")]
        [SerializeField] protected SharedVariable<float> m_Radius = 5f;
        [Tooltip("The angle offset in degrees for the first agent.")]
        [SerializeField] protected SharedVariable<float> m_AngleOffset = 0f;

        /// <summary>
        /// Calculate the position for this agent in the circle formation.
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

            // Calculate the position in local space.
            var angle = m_AngleOffset.Value + (360f / totalAgents) * index;
            var angleRad = angle * Mathf.Deg2Rad;
            Vector3 localPosition;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(Mathf.Sin(angleRad) * m_Radius.Value, Mathf.Cos(angleRad) * m_Radius.Value, 0);
            } else {
                // Use the XZ plane for 3D.
                localPosition = new Vector3(Mathf.Sin(angleRad) * m_Radius.Value, 0, Mathf.Cos(angleRad) * m_Radius.Value);
            }

            // Calculate the agent's position.
            var position = center + localPosition;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            return position;
        }
    }
} 
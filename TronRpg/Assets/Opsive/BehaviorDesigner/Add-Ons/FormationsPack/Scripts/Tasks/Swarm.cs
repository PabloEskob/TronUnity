/// ---------------------------------------------
/// Formations Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.FormationsPack.Runtime.Tasks
{
    using Opsive.BehaviorDesigner.AddOns.Shared.Runtime;
    using Opsive.BehaviorDesigner.AddOns.Shared.Runtime.Tasks;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using UnityEngine;
    using Unity.Entities;

    [Opsive.Shared.Utility.Description("Moves agents in a swarm formation.")]
    [NodeIcon("1d9c62a500bf7f647be16de3bff46c1e", "9f65f0b4b4d76424ebf99dc478e5c117")]
    public class Swarm : FormationsTargetBase
    {
        /// <summary>
        /// Specifies how the swarm should be distributed.
        /// </summary>
        public enum SwarmDistribution
        {
            Uniform,    // Uniform distribution within the radius.
            Gaussian    // Gaussian distribution within the radius.
        }

        [Tooltip("The radius of the swarm.")]
        [SerializeField] protected SharedVariable<float> m_Radius = 5f;
        [Tooltip("The minimum distance between agents.")]
        [SerializeField] protected SharedVariable<float> m_MinDistance = 1f;
        [Tooltip("Specifies how the swarm should be distributed.")]
        [SerializeField] protected SharedVariable<SwarmDistribution> m_Distribution = SwarmDistribution.Uniform;
        [Tooltip("The seed for random number generation.")]
        [SerializeField] protected SharedVariable<int> m_Seed = -1;

        private Vector3? m_CachedPosition;

        public override bool AssignOptimialIndicies => false;

        /// <summary>
        /// The task has been initialized.
        /// </summary>
        public override void OnAwake()
        {
            base.OnAwake();

            if (m_Seed.Value != -1) {
                Random.InitState(m_Seed.Value);
            }
        }

        /// <summary>
        /// Calculate the position for this agent in the swarm formation.
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

            // Return cached position if it exists.
            if (m_CachedPosition.HasValue) {
                return center + m_CachedPosition.Value;
            }

            // Generate random position within the radius.
            float distance;
            if (m_Distribution.Value == SwarmDistribution.Gaussian) {
                // Use Box-Muller transform for Gaussian distribution.
                var u1 = Random.value;
                var u2 = Random.value;
                var z0 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
                distance = Mathf.Clamp01(Mathf.Abs(z0) / 3.0f) * m_Radius.Value;
            } else {
                // Uniform distribution.
                distance = Random.value * m_Radius.Value;
            }
            var angle = Random.value * 2f * Mathf.PI;

            Vector3 localPosition;
            if (m_Is2D) {
                // Use the XY plane for 2D.
                localPosition = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0);
            } else {
                // Use the XZ plane for 3D.
                localPosition = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
            }

            // Ensure minimum distance from other agents.
            if (m_MinDistance.Value > 0) {
                var minDistanceSqr = m_MinDistance.Value * m_MinDistance.Value;
                var attempts = 0;
                var group = FormationsManager.GetFormationGroup(m_FormationGroupID.Value);
                while (attempts < 10) {
                    var tooClose = false;
                    for (int i = 0; i < index; ++i) {
                        if (group.Members[i].FormationIndex == -1) {
                            continue;
                        }

                        var otherPos = group.Members[i].DesiredPosition;
                        if ((otherPos - (center + localPosition)).sqrMagnitude < minDistanceSqr) {
                            tooClose = true;
                            break;
                        }
                    }
                    if (!tooClose) break;

                    // Try a new position.
                    distance = Random.value * m_Radius.Value;
                    angle = Random.value * 2f * Mathf.PI;
                    if (m_Is2D) {
                        localPosition = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0);
                    } else {
                        localPosition = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
                    }
                    attempts++;
                }
            }

            // Calculate the agent's position.
            var position = center + localPosition;
            var validPos = position;
            if (samplePosition && SamplePosition(ref validPos)) {
                position = validPos;
            }

            // Cache the local position to allow for multiple calls to CalculateFormationPosition.
            m_CachedPosition = position - center;

            return position;
        }

        /// <summary>
        /// The task has ended.
        /// </summary>
        public override void OnEnd()
        {
            base.OnEnd();
            m_CachedPosition = null;
        }

        /// <summary>
        /// Data structure for saving swarm formation state.
        /// </summary>
        private class SwarmSaveData
        {
            [Tooltip("The base formation save data containing common formation state.")]
            public FormationSaveData BaseData;
            [Tooltip("Indicates if the agent has a valid cached position.")]
            public bool HasCachedPosition;
            [Tooltip("The cached position offset from the formation center for this agent.")]
            public Vector3 CachedPosition;
        }

        /// <summary>
        /// Returns the current task state.
        /// </summary>
        /// <param name="world">The DOTS world.</param>
        /// <param name="entity">The DOTS entity.</param>
        /// <returns>The current task state.</returns>
        public override object Save(World world, Entity entity)
        {
            var saveData = base.Save(world, entity);
            if (saveData == null) {
                return null;
            }

            var swarmSaveData = new SwarmSaveData();
            swarmSaveData.BaseData = (FormationSaveData)saveData;
            if (m_CachedPosition.HasValue) {
                swarmSaveData.HasCachedPosition = true;
                swarmSaveData.CachedPosition = m_CachedPosition.Value;
            }
            return swarmSaveData;
        }

        /// <summary>
        /// Loads the previous task state.
        /// </summary>
        /// <param name="saveData">The previous task state.</param>
        /// <param name="world">The DOTS world.</param>
        /// <param name="entity">The DOTS entity.</param>
        public override void Load(object saveData, World world, Entity entity)
        {
            if (saveData == null) {
                return;
            }

            var swarmSaveData = (SwarmSaveData)saveData;
            base.Load(swarmSaveData.BaseData, world, entity);
            if (swarmSaveData.HasCachedPosition) {
                m_CachedPosition = swarmSaveData.CachedPosition;
            }
        }

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_Radius = 5f;
            m_MinDistance = 1f;
            m_Distribution = SwarmDistribution.Uniform;
            m_Seed = -1;
        }
    }
} 
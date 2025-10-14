/// ---------------------------------------------
/// Formations Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.FormationsPack.Demo
{
    using Opsive.BehaviorDesigner.AddOns.Shared.Demo;
    using Opsive.BehaviorDesigner.Runtime;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// A Formations Scenario Selector that sets the leader formation agent.
    /// </summary>
    public class FormationsScenarioSelector : ScenarioSelector
    {
        /// <summary>
        /// Specifies a behavior tree agent index and its corresponding start delay.
        /// </summary>
        [System.Serializable]
        public struct DelayedAgent
        {
            [Tooltip("The index of the agent to delay.")]
            public int Index;
            [Tooltip("The delay in seconds before starting the behavior tree.")]
            public float Delay;
            [Tooltip("A reference to the active coroutine.")]
            [System.NonSerialized] public Coroutine ActiveCoroutine;
        }

        /// <summary>
        /// Maps a behavior tree index to a specific formation group.
        /// </summary>
        [System.Serializable]
        public struct FormationGroupAssignment
        {
            [Tooltip("The index of the behavior tree to assign to a group.")]
            public int Index;
            [Tooltip("The formation group to assign the behavior tree to.")]
            public int FormationGroup;
            [Tooltip("The delay in seconds before assigning the group.")]
            public float Delay;
            [Tooltip("The destination to move towards.")]
            public Vector3 Destination;
            [Tooltip("A reference to the active coroutine.")]
            [System.NonSerialized] public Coroutine ActiveCoroutine;
        }

        [Tooltip("The name of the force leader SharedVariable.")]
        [SerializeField] protected string m_ForceLeaderVariableName = "IsLeader";
        [Header("Delayed Start")]
        [Tooltip("The agents that should be delayed.")]
        [SerializeField] protected DelayedAgent[] m_DelayedAgents;
        [Header("Formation Groups")]
        [Tooltip("The formation group assignments.")]
        [SerializeField] protected FormationGroupAssignment[] m_GroupAssignments;
        [Tooltip("The name of the formation group variable in the behavior tree.")]
        [SerializeField] protected string m_FormationGroupVariableName = "FormationGroup";
        [Tooltip("The name of the destination variable in the behavior tree.")]
        [SerializeField] protected string m_DestinationVariableName = "Destination";

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // The leader will always be the first agent.
            for (int i = 0; i < m_AgentBehaviorTrees.Length; ++i) {
                m_AgentBehaviorTrees[i][0].GetVariable<bool>(m_ForceLeaderVariableName, GraphDesigner.Runtime.Variables.SharedVariable.SharingScope.GameObject).Value = true;
            }
        }

        /// <summary>
        /// Apply the delayed start setup to the behavior trees.
        /// </summary>
        public void StartDelayedStartScenario()
        {
            for (int i = 0; i < m_DelayedAgents.Length; ++i) {
                var delayedAgent = m_DelayedAgents[i];
                var behaviorTree = m_AgentBehaviorTrees[delayedAgent.Index][m_ActiveIndex];
                m_DelayedAgents[i].ActiveCoroutine = StartCoroutine(DelayedStart(behaviorTree, delayedAgent.Delay));
            }
        }

        /// <summary>
        /// Stop all delayed start coroutines.
        /// </summary>
        public void StopDelayedStartScenario()
        {
            for (int i = 0; i < m_DelayedAgents.Length; ++i) {
                if (m_DelayedAgents[i].ActiveCoroutine != null) {
                    StopCoroutine(m_DelayedAgents[i].ActiveCoroutine);
                    m_DelayedAgents[i].ActiveCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Coroutine that delays the start of a behavior tree.
        /// </summary>
        /// <param name="behaviorTree">The behavior tree to delay.</param>
        /// <param name="delay">The delay in seconds.</param>
        private IEnumerator DelayedStart(BehaviorTree behaviorTree, float delay)
        {
            behaviorTree.enabled = false;
            yield return new WaitForSeconds(delay);
            behaviorTree.enabled = true;
        }

        /// <summary>
        /// Apply the formation group setup to the behavior trees.
        /// </summary>
        public void StartFormationGroupScenario()
        {
            for (int i = 0; i < m_GroupAssignments.Length; ++i) {
                var assignment = m_GroupAssignments[i];
                var behaviorTree = m_AgentBehaviorTrees[assignment.Index][m_ActiveIndex];
                m_GroupAssignments[i].ActiveCoroutine = StartCoroutine(AssignGroup(behaviorTree, assignment.Delay, assignment.FormationGroup, assignment.Destination));
            }
        }

        /// <summary>
        /// Stop all formation group coroutines.
        /// </summary>
        public void StopFormationGroupScenario()
        {
            for (int i = 0; i < m_GroupAssignments.Length; ++i) {
                if (m_GroupAssignments[i].ActiveCoroutine != null) {
                    StopCoroutine(m_GroupAssignments[i].ActiveCoroutine);
                    m_GroupAssignments[i].ActiveCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Coroutine that assigns the group after a delay.
        /// </summary>
        /// <param name="behaviorTree">The behavior tree to assign.</param>
        /// <param name="delay">The delay in seconds.</param>
        /// <param name="groupID">The ID of the group.</param>
        /// <param name="destination">The destination of the group.</param>
        private IEnumerator AssignGroup(BehaviorTree behaviorTree, float delay, int groupID, Vector3 destination)
        {
            yield return new WaitForSeconds(delay);
            var formationGroupVariable = behaviorTree.GetVariable<int>(m_FormationGroupVariableName);
            if (formationGroupVariable != null) {
                formationGroupVariable.Value = groupID;
            }
            var destinationVariable = behaviorTree.GetVariable<Vector3>(m_DestinationVariableName);
            if (destinationVariable != null) {
                destinationVariable.Value = destination;
            }
        }
    }
} 
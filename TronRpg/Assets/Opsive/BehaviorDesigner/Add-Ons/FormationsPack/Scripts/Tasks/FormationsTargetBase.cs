/// ---------------------------------------------
/// Formations Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.Shared.Runtime.Tasks
{
    using Opsive.GraphDesigner.Runtime.Variables;
    using UnityEngine;

    /// <summary>
    /// Implements a target position for the Formations group.
    /// </summary>
    public abstract class FormationsTargetBase : FormationsBase
    {
        [Tooltip("The target position for the formation.")]
        [SerializeField] protected SharedVariable<Vector3> m_TargetPosition;

        public override Vector3 TargetPosition => m_TargetPosition.Value;

        /// <summary>
        /// The task has been initialized.
        /// </summary>
        public override void OnAwake()
        {
            base.OnAwake();

            m_TargetPosition.OnValueChange += TargetPositionChanged;
        }

        /// <summary>
        /// The TargetPosition SharedVariable has updated.
        /// </summary>
        private void TargetPositionChanged()
        {
            if (m_Group == null || m_Group.Leader != this) {
                return;
            }

            // The position only needs to update if the agents are already moving to the target.
            if (m_Group.State == FormationsManager.FormationState.MoveToTarget) {
                m_Group.TargetPosition = TargetPosition;
                // Update all agents' destinations
                for (int i = 0; i < m_Group.Members.Count; ++i) {
                    if (m_Group.Members[i].FormationIndex == -1) {
                        continue;
                    }
                    m_Group.Members[i].DesiredPosition = m_Group.Members[i].UpdateFormationDestination();
                }
            }
        }

        /// <summary>
        /// The behavior tree has been destroyed.
        /// </summary>
        public override void OnDestroy()
        {
            m_TargetPosition.OnValueChange -= TargetPositionChanged;
        }

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_TargetPosition = Vector3.zero;
        }
    }
}
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Actions
{
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using MoreMountains.Feedbacks;
    using UnityEngine;

    /// <summary>
    /// Plays all feedbacks on the specified MMF_Player.
    /// </summary>
    [NodeIcon("ff784a0f45715d54fba0bc7066fb7fc4")]
    [NodeDescription("Plays all feedbacks on the specified MMF_Player.")]
    [Shared.Utility.Category("Feel")]
    public class PlayFeedbacks : TargetGameObjectAction
    {
        [Tooltip("The position where the feedbacks should be played. Uses the agent's position if zero.")]
        [SerializeField] protected SharedVariable<Vector3> m_Position;
        [Tooltip("The intensity multiplier for the feedbacks.")]
        [SerializeField] protected SharedVariable<float> m_Intensity = -1;
        [Tooltip("Should the feedbacks be reverted to their starting values?")]
        [SerializeField] protected SharedVariable<bool> m_Revert = false;

        private MMF_Player m_ResolvedPlayer;

        /// <summary>
        /// Initializes the target GameObject.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            
            m_ResolvedPlayer = m_ResolvedGameObject.GetComponent<MMF_Player>();
        }

        /// <summary>
        /// Executes the task logic.
        /// </summary>
        /// <returns>The status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_ResolvedPlayer == null) {
                Debug.LogWarning($"Error: No MMF_Player found on GameObject {m_ResolvedGameObject.name}.");
                return TaskStatus.Failure;
            }

            var playPosition = m_Position.Value;
            if (m_Position.Value == Vector3.zero) {
                playPosition = m_ResolvedTransform.position;
            }

            m_ResolvedPlayer.PlayFeedbacks(playPosition, m_Intensity.Value, m_Revert.Value);
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_Position = Vector3.zero;
            m_Intensity = -1;
            m_Revert = false;
        }
    }
}

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
    /// Plays a specific feedback on the specified MMF_Player.
    /// </summary>
    [NodeIcon("ff784a0f45715d54fba0bc7066fb7fc4")]
    [NodeDescription("Plays a specific feedback on the specified MMF_Player.")]
    [Shared.Utility.Category("Feel")]
    public class PlayFeedback : TargetGameObjectAction
    {
        [Tooltip("The index of the feedback to play in the player's feedback list.")]
        [SerializeField] protected SharedVariable<int> m_FeedbackIndex;
        [Tooltip("The label/name of the feedback to play. If specified, will override the index.")]
        [SerializeField] protected SharedVariable<string> m_FeedbackLabel;
        [Tooltip("The position where the feedbacks should be played. Uses the agent's position if zero.")]
        [SerializeField] protected SharedVariable<Vector3> m_Position;
        [Tooltip("The intensity multiplier for the feedback (0-1).")]
        [SerializeField] protected SharedVariable<float> m_Intensity = -1;

        private MMF_Player m_ResolvedPlayer;
        private MMF_Feedback m_TargetFeedback;

        /// <summary>
        /// Initializes the target GameObject.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            
            m_ResolvedPlayer = m_ResolvedGameObject.GetComponent<MMF_Player>();
        }

        /// <summary>
        /// The task has started.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();
            
            m_TargetFeedback = FindTargetFeedback();
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

            if (m_TargetFeedback == null) {
                Debug.LogWarning($"Error: No target feedback found to play on GameObject {m_ResolvedGameObject.name}.");
                return TaskStatus.Failure;
            }

            var playPosition = m_Position.Value;
            if (m_Position.Value == Vector3.zero) {
                playPosition = m_ResolvedTransform.position;
            }

            m_TargetFeedback.Play(playPosition, m_Intensity.Value);
            return TaskStatus.Success;
        }

        /// <summary>
        /// Finds the target feedback based on index or label.
        /// </summary>
        /// <returns>The target feedback (can be null).</returns>
        private MMF_Feedback FindTargetFeedback()
        {
            if (m_ResolvedPlayer == null || m_ResolvedPlayer.FeedbacksList == null) {
                return null;
            }

            // Search by the label if a label is specified.
            if (!string.IsNullOrEmpty(m_FeedbackLabel.Value))
            {
                for (int i = 0; i < m_ResolvedPlayer.FeedbacksList.Count; ++i) {
                    var feedback = m_ResolvedPlayer.FeedbacksList[i];
                    if (feedback != null && feedback.Label == m_FeedbackLabel.Value) {
                        return feedback;
                    }
                }
                
                Debug.LogWarning($"Error: No feedback found with label {m_FeedbackLabel.Value} on GameObject {m_ResolvedGameObject.name}.");
                return null;
            }

            if (m_FeedbackIndex.Value >= 0 && m_FeedbackIndex.Value < m_ResolvedPlayer.FeedbacksList.Count) {
                return m_ResolvedPlayer.FeedbacksList[m_FeedbackIndex.Value];
            }
            
            Debug.LogWarning($"Error: Feedback index {m_FeedbackIndex.Value} is out of range. Available feedbacks: {m_ResolvedPlayer.FeedbacksList.Count} on GameObject {m_ResolvedGameObject.name}.");
            return null;
        }

        /// <summary>
        /// Resets the task values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_FeedbackIndex = 0;
            m_FeedbackLabel = string.Empty;
            m_Position = Vector3.zero;
            m_Intensity = -1;
        }
    }
}

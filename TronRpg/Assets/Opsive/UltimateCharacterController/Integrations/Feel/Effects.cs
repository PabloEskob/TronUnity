/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.UltimateCharacterController.Integrations.Feel
{
    using Opsive.UltimateCharacterController.Character.Effects;
    using MoreMountains.Feedbacks;
    using UnityEngine;

    /// <summary>
    /// Plays the MMF_Player feedbacks.
    /// </summary>
    public class PlayFeedbacks : Effect
    {
        [Tooltip("A reference to the feedbacks that should be played.")]
        [SerializeField] protected MMF_Player m_Player;
        [Tooltip("The intensity of the feedbacks.")]
        [SerializeField] protected float m_Intensity = -1;
        [Tooltip("Should the feedbacks be reverted to their starting values?")]
        [SerializeField] protected bool m_Revert = false;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            m_Player = FeelUtility.InitializePlayer(m_Player, m_GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedback effect.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Starts the effect.
        /// </summary>
        public override void Start()
        {
            base.Start();

            FeelUtility.PlayFeedbacks(m_Player, m_Transform.position, m_Intensity, m_Revert);
        }
    }

    /// <summary>
    /// Plays the MMF_Player feedbacks.
    /// </summary>
    public class PlayFeedback : Effect
    {
        [Tooltip("A reference to the feedbacks that should be played.")]
        [SerializeField] protected MMF_Player m_Player;
        [Tooltip("The index of the feedback to play in the player's feedback list.")]
        [SerializeField] protected int m_FeedbackIndex;
        [Tooltip("The label/name of the feedback to play. If specified, will override the index.")]
        [SerializeField] protected string m_FeedbackLabel;
        [Tooltip("The intensity of the feedbacks.")]
        [SerializeField] protected float m_Intensity = -1;

        private System.Type m_FeedbackType;
        private MMFeedback m_Feedback;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            m_Player = FeelUtility.InitializePlayer(m_Player, m_GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbackType effect.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Starts the effect.
        /// </summary>
        public override void Start()
        {
            base.Start();

            m_Feedback = FeelUtility.PlayFeedback(m_Player, m_Feedback, m_FeedbackIndex, m_FeedbackLabel, m_Transform.position, m_Intensity);
        }
    }
}
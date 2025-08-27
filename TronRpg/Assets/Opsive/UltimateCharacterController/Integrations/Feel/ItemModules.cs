/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.UltimateCharacterController.Integrations.Feel
{
    using Opsive.UltimateCharacterController.Items.Actions;
    using Opsive.UltimateCharacterController.Items.Actions.Modules;
    using Opsive.UltimateCharacterController.Items.Actions.Modules.Magic;
    using Opsive.UltimateCharacterController.Items.Actions.Modules.Melee;
    using Opsive.UltimateCharacterController.Items.Actions.Modules.Shootable;
    using Opsive.UltimateCharacterController.Items.Actions.Modules.Throwable;
    using MoreMountains.Feedbacks;
    using UnityEngine;
    using System;

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbacksShootableModule : ShootableExtraModule, IModuleUseItem
    {
        [Tooltip("A reference to the feedbacks that should be played.")]
        [SerializeField] protected MMF_Player m_Player;
        [Tooltip("The intensity of the feedbacks.")]
        [SerializeField] protected float m_Intensity = -1;
        [Tooltip("Should the feedbacks be reverted to their starting values?")]
        [SerializeField] protected bool m_Revert = false;

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            FeelUtility.PlayFeedbacks(m_Player, Transform.position, m_Intensity, m_Revert);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbackShootableModule : ShootableExtraModule, IModuleUseItem
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
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            m_Feedback = FeelUtility.PlayFeedback(m_Player, m_Feedback, m_FeedbackIndex, m_FeedbackLabel, Transform.position, m_Intensity);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbacksMeleeModule : MeleeExtraModule, IModuleUseItem
    {
        [Tooltip("A reference to the feedbacks that should be played.")]
        [SerializeField] protected MMF_Player m_Player;
        [Tooltip("The intensity of the feedbacks.")]
        [SerializeField] protected float m_Intensity = -1;
        [Tooltip("Should the feedbacks be reverted to their starting values?")]
        [SerializeField] protected bool m_Revert = false;

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            FeelUtility.PlayFeedbacks(m_Player, Transform.position, m_Intensity, m_Revert);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbackMeleeModule : MeleeExtraModule, IModuleUseItem
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
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            m_Feedback = FeelUtility.PlayFeedback(m_Player, m_Feedback, m_FeedbackIndex, m_FeedbackLabel, Transform.position, m_Intensity);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbacksMagicModule : MagicExtraModule, IModuleUseItem
    {
        [Tooltip("A reference to the feedbacks that should be played.")]
        [SerializeField] protected MMF_Player m_Player;
        [Tooltip("The intensity of the feedbacks.")]
        [SerializeField] protected float m_Intensity = -1;
        [Tooltip("Should the feedbacks be reverted to their starting values?")]
        [SerializeField] protected bool m_Revert = false;

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            FeelUtility.PlayFeedbacks(m_Player, Transform.position, m_Intensity, m_Revert);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbackMagicModule : MagicExtraModule, IModuleUseItem
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
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            m_Feedback = FeelUtility.PlayFeedback(m_Player, m_Feedback, m_FeedbackIndex, m_FeedbackLabel, Transform.position, m_Intensity);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbacksThrowableModule : ThrowableExtraModule, IModuleUseItem
    {
        [Tooltip("A reference to the feedbacks that should be played.")]
        [SerializeField] protected MMF_Player m_Player;
        [Tooltip("The intensity of the feedbacks.")]
        [SerializeField] protected float m_Intensity = -1;
        [Tooltip("Should the feedbacks be reverted to their starting values?")]
        [SerializeField] protected bool m_Revert = false;

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            FeelUtility.PlayFeedbacks(m_Player, Transform.position, m_Intensity, m_Revert);
        }
    }

    /// <summary>
    /// Plays the feedbacks when the item is used.
    /// </summary>
    [Serializable]
    public class PlayFeedbackThrowableModule : ThrowableExtraModule, IModuleUseItem
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
        /// Initialize the module.
        /// </summary>
        /// <param name="itemAction">The parent item action.</param>
        protected override void Initialize(CharacterItemAction itemAction)
        {
            base.Initialize(itemAction);

            m_Player = FeelUtility.InitializePlayer(m_Player, GameObject);
            if (m_Player == null) {
                Debug.LogError("Error: The MMF_Player component must be assigned on the PlayFeedbacksModule.");
                Enabled = false;
                return;
            }
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        public void UseItem()
        {
            m_Feedback = FeelUtility.PlayFeedback(m_Player, m_Feedback, m_FeedbackIndex, m_FeedbackLabel, Transform.position, m_Intensity);
        }
    }
}
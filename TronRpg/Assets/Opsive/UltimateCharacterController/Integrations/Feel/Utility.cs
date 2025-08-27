namespace Opsive.UltimateCharacterController.Integrations.Feel
{
    using MoreMountains.Feedbacks;
    using UnityEngine;

    /// <summary>
    /// Utility functions for the Feel integration.
    /// </summary>
    public static class FeelUtility
    {
        /// <summary>
        /// Initializes the Feel player.
        /// </summary>
        /// <param name="player">A reference to the existing MMF_Player.</param>
        /// <param name="gameObject">The GameObject that the player should be attached to.</param>
        /// <returns>The initialized player (can be null).</returns>
        public static MMF_Player InitializePlayer(MMF_Player player, GameObject gameObject)
        {
            if (player == null) {
                player = gameObject.GetComponent<MMF_Player>();
                if (player == null) { return null; }
            }

            player.Initialization();
            return player;
        }

        /// <summary>
        /// Plays the feedbacks on the specified player.
        /// </summary>
        /// <param name="player">The player that the feedbacks should be played on.</param>
        /// <param name="position">The position if used.</param>
        /// <param name="intensity">The intensity of the feedbacks.</param>
        /// <param name="revert">Should the feedbacks be reverted?</param>
        public static void PlayFeedbacks(MMF_Player player, Vector3 position, float intensity, bool revert)
        {
            player.PlayFeedbacks(position, intensity, revert);
        }

        /// <summary>
        /// Plays the specified feedback.
        /// </summary>
        /// <param name="player">The player that the feedback should be played on.</param>
        /// <param name="feedback">The feedback that should be played.</param>
        /// <param name="index">The index of the feedback to play in the player's feedback list.</param>
        /// <param name="label">The label/name of the feedback to play. If specified, will override the index.</param>
        /// <param name="position">The position of the feedback.</param>
        /// <param name="intensity">The intensity of the feedback.</param>
        /// <returns>The retrieved feedback (can be null).</returns>
        public static MMFeedback PlayFeedback(MMF_Player player, MMFeedback feedback, int index, string label, Vector3 position, float intensity)
        {
            if (feedback == null) {
                var feedbacks = player.Feedbacks;
                if (feedbacks != null) {
                    if (!string.IsNullOrEmpty(label)) {
                        for (int i = 0; i < feedbacks.Count; ++i) {
                            if (feedbacks[i] != null && feedbacks[i].Label == label) {
                                feedback = feedbacks[i];
                                break;
                            }
                        }
                    }

                    if (index >= 0 && index < feedbacks.Count) {
                        feedback = feedbacks[index];
                    }
                }
            }

            if (feedback == null) {
                Debug.LogError($"Error: Unable to find the feedback with index {index} or label {label}.");
                return null;
            }

            feedback.Play(position, intensity);
            return feedback;
        }
    }
}
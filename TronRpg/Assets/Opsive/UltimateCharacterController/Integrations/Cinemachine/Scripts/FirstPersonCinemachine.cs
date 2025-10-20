/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Integrations.Cinemachine
{
    using Opsive.Shared.Events;
    using Opsive.Shared.Game;
    using Opsive.UltimateCharacterController.Game;
    using Opsive.UltimateCharacterController.Utility;
    using UnityEngine;
    using static Opsive.UltimateCharacterController.FirstPersonController.Camera.ViewTypes.FirstPerson;

    /// <summary>
    /// Implements a First Person ViewType for the Cinemachine ViewType.
    /// </summary>
    [Shared.StateSystem.AddState("Aim", "b14582acc4cb55749ba660a8c8ff4638")]
    public class FirstPersonCinemachine : CinemachineViewType
    {
        public override bool FirstPersonPerspective { get { return true; } }

        [Tooltip("The culling mask of the camera.")]
        [SerializeField] protected LayerMask m_CullingMask = ~(1 << LayerManager.Overlay);
        [Tooltip("Specifies how the overlay objects are rendered.")]
        [SerializeField] protected ObjectOverlayRenderType m_OverlayRenderType = ObjectOverlayRenderType.RenderPipeline;
        [Tooltip("A reference to the first person camera.")]
        [SerializeField] protected UnityEngine.Camera m_FirstPersonCamera;
        [Tooltip("The culling mask of the first person objects.")]
        [SerializeField] protected LayerMask m_FirstPersonCullingMask = 1 << LayerManager.Overlay;
        [Tooltip("The minimum pitch angle (in degrees).")]
        [SerializeField] protected float m_MinPitchLimit = -72;
        [Tooltip("The maximum pitch angle (in degrees).")]
        [SerializeField] protected float m_MaxPitchLimit = 72;
        [Tooltip("The rate that the camera changes its position while the character is moving.")]
        [SerializeField] protected Vector3 m_BobPositionalRate = new Vector3(0.0f, 1.4f, 0.0f);
        [Tooltip("The strength of the positional camera bob. Determines how far the camera swings in each respective direction.")]
        [SerializeField] protected Vector3 m_BobPositionalAmplitude = new Vector3(0.0f, 0.35f, 0.0f);
        [Tooltip("The rate that the camera changes its roll rotation value while the character is moving.")]
        [SerializeField] protected float m_BobRollRate = 0.9f;
        [Tooltip("The strength of the roll within the camera bob. Determines how far the camera tilts from left to right.")]
        [SerializeField] protected float m_BobRollAmplitude = 1.7f;
        [Tooltip("This tweaking feature is useful if the bob motion gets out of hand after changing character velocity.")]
        [SerializeField] protected float m_BobInputVelocityScale = 1;
        [Tooltip("A cap on the velocity value from the bob function, preventing the camera from flipping out when the character travels at excessive speeds.")]
        [SerializeField] protected float m_BobMaxInputVelocity = 1000;
        [Tooltip("A trough should only occur when the bob vertical offset is less then the specified value.")]
        [SerializeField] protected float m_BobMinTroughVerticalOffset = -0.01f;
        [Tooltip("The amount of force to add when the bob has reached its lowest point. This can be used to add a shaking effect to the camera to mimick a giant walking.")]
        [SerializeField] protected Vector3 m_BobTroughForce = new Vector3(0.0f, 0.0f, 0.0f);
        [Tooltip("Determines whether the bob should stay in effect only when the character is on the ground.")]
        [SerializeField] protected bool m_BobRequireGroundContact = true;

        protected float m_Yaw;

        private float m_PrevBobSpeed;
        private float m_BobVerticalOffset = float.MaxValue;
        private bool m_BobVerticalOffsetDecreasing;
        private float m_TargetVerticalOffsetAdjustment;

        public override float Yaw { get { return m_Yaw; } }
        protected override float VerticalOffsetAdjustment => m_TargetVerticalOffsetAdjustment;

        public override void Awake()
        {
            base.Awake();

            if (m_OverlayRenderType != ObjectOverlayRenderType.None) {
                m_Camera.cullingMask &= m_CullingMask;
            }
        }

        public override void AttachCharacter(GameObject character)
        {
            // Unregister from any events on the previous character.
            if (m_Character != null) {
                EventHandler.UnregisterEvent<bool>(m_Character, "OnCameraChangePerspectives", UpdateFirstPersonCamera);
                EventHandler.UnregisterEvent<float>(m_Character, "OnHeightChangeAdjustHeight", AdjustVerticalOffset);
            }

            base.AttachCharacter(character);

            // Initialize the camera with the new character.
            if (m_Character != null) {
                EventHandler.RegisterEvent<bool>(m_Character, "OnCameraChangePerspectives", UpdateFirstPersonCamera);

                Animator characterAnimator;
                var modelManager = m_Character.GetCachedComponent<UltimateCharacterController.Character.ModelManager>();
                if (modelManager != null) {
                    characterAnimator = modelManager.ActiveModel.GetCachedComponent<Animator>();
                } else {
                    characterAnimator = m_Character.GetComponentInChildren<Animator>();
                }
                if (characterAnimator == null) {
                    EventHandler.RegisterEvent<float>(m_Character, "OnHeightChangeAdjustHeight", AdjustVerticalOffset);
                }
            }
        }

        /// <summary>
        /// The view type has changed.
        /// </summary>
        /// <param name="activate">Should the current view type be activated?</param>
        /// <param name="pitch">The pitch of the camera (in degrees).</param>
        /// <param name="yaw">The yaw of the camera (in degrees).</param>
        /// <param name="characterRotation">The rotation of the character.</param>
        public override void ChangeViewType(bool activate, float pitch, float yaw, Quaternion characterRotation)
        {
            base.ChangeViewType(activate, pitch, yaw, characterRotation);

            if (activate) {
                m_Pitch = pitch;
                m_Yaw = yaw;

                UpdateFirstPersonCamera(m_CharacterLocomotion.FirstPersonPerspective);
            }
        }

        /// <summary>
        /// Updates the first person camera and culling mask depending on if the first person camera is in use.
        /// </summary>
        /// <param name="firstPersonPerspective">Is the character in a first person perspective?</param>
        private void UpdateFirstPersonCamera(bool firstPersonPerspective)
        {
            if (m_OverlayRenderType == ObjectOverlayRenderType.None) {
                return;
            }

            if (firstPersonPerspective) {
                if (m_FirstPersonCamera != null) {
                    m_FirstPersonCamera.gameObject.SetActive(true);
                }
                if (m_OverlayRenderType == ObjectOverlayRenderType.RenderPipeline) {
                    m_Camera.cullingMask |= m_FirstPersonCullingMask;
                }
            } else {
                if (m_FirstPersonCamera != null) {
                    m_FirstPersonCamera.gameObject.SetActive(false);
                }
                if (m_OverlayRenderType == ObjectOverlayRenderType.RenderPipeline) {
                    m_Camera.cullingMask &= ~m_FirstPersonCullingMask;
                }
            }
        }

        /// <summary>
        /// Reset the ViewType's variables.
        /// </summary>
        /// <param name="characterRotation">The rotation of the character.</param>
        public override void Reset(Quaternion characterRotation)
        {
            base.Reset(characterRotation);

            m_Pitch = 0;
            m_Yaw = 0;
            m_BobVerticalOffset = float.MaxValue;
            m_BobVerticalOffsetDecreasing = false;
            m_PrevBobSpeed = 0;
        }

        /// <summary>
        /// Rotates the camera according to the horizontal and vertical movement values.
        /// </summary>
        /// <param name="horizontalMovement">-1 to 1 value specifying the amount of horizontal movement.</param>
        /// <param name="verticalMovement">-1 to 1 value specifying the amount of vertical movement.</param>
        /// <param name="immediateUpdate">Should the camera be updated immediately?</param>
        /// <returns>The updated rotation.</returns>
        public override Quaternion Rotate(float horizontalMovement, float verticalMovement, bool immediateUpdate)
        {
            // Update the rotation. The pitch may have a limit.
            if (Mathf.Abs(m_MinPitchLimit - m_MaxPitchLimit) < 180) {
                m_Pitch = MathUtility.ClampAngle(m_Pitch, -verticalMovement, m_MinPitchLimit, m_MaxPitchLimit);
            } else {
                m_Pitch -= verticalMovement;
            }
            m_Yaw += horizontalMovement;

            // Prevent the values from getting too large.
            m_Pitch = MathUtility.ClampInnerAngle(m_Pitch);
            m_Yaw = MathUtility.ClampInnerAngle(m_Yaw);

            return base.Rotate(horizontalMovement, verticalMovement, immediateUpdate);
        }

        /// <summary>
        /// Moves the camera according to the current pitch and yaw values.
        /// </summary>
        /// <param name="immediateUpdate">Should the camera be updated immediately?</param>
        /// <returns>The updated position.</returns>
        public override Vector3 Move(bool immediateUpdate)
        {
            UpdateBob();

            return base.Move(immediateUpdate);
        }

        /// <summary>
        /// Updates the first person bob.
        /// </summary>
        private void UpdateBob()
        {
            if ((m_BobPositionalRate == Vector3.zero || m_BobPositionalAmplitude == Vector3.zero) && (m_BobRollRate == 0 || m_BobRollAmplitude == 0)) {
                return;
            }

            var bobSpeed = ((m_BobRequireGroundContact && !m_CharacterLocomotion.Grounded) ? 0 : m_CharacterLocomotion.Velocity.sqrMagnitude);

            // Scale and limit the input velocity.
            bobSpeed = Mathf.Min(bobSpeed * m_BobInputVelocityScale, m_BobMaxInputVelocity);

            // Reduce the number of decimals to avoid floating point imprecision issues.
            bobSpeed = Mathf.Round(bobSpeed * 1000) / 1000;

            // If the bob speed is zero then fade out the last speed value. It is important to clamp the speed to the 
            // last bob speed value because a preset may have changed since the last last bob.
            if (bobSpeed == 0) {
                bobSpeed = Mathf.Min((m_PrevBobSpeed * 0.93f), m_BobMaxInputVelocity);
            }

            // Update the positional and roll bob value.
            var currentPositionalBobAmplitude = (bobSpeed * (m_BobPositionalAmplitude * -0.01f));
            var currentRollBobAmplitude = (bobSpeed * (m_BobRollAmplitude * -0.01f));
            Vector3 currentBobOffsetValue;
            currentBobOffsetValue.x = Mathf.Cos(m_BobPositionalRate.x * m_CharacterLocomotion.TimeScale * Time.time * 10) * currentPositionalBobAmplitude.x;
            currentBobOffsetValue.y = Mathf.Cos(m_BobPositionalRate.y * m_CharacterLocomotion.TimeScale * Time.time * 10) * currentPositionalBobAmplitude.y;
            currentBobOffsetValue.z = Mathf.Cos(m_BobPositionalRate.z * m_CharacterLocomotion.TimeScale * Time.time * 10) * currentPositionalBobAmplitude.z;
            var currentBobRollValue = Mathf.Cos(m_BobRollRate * m_CharacterLocomotion.TimeScale * Time.time * 10) * currentRollBobAmplitude;

            // Add the bob value to the positional and rotational spring.
            m_PositionSpring.AddForce(currentBobOffsetValue);
            m_RotationSpring.AddForce(Vector3.forward * currentBobRollValue);
            m_PrevBobSpeed = bobSpeed;

            // Detect if the bob was previously decreasing and is now moving back up. This will indicate that the bob was at the lowest position.
            if (currentBobOffsetValue.y < m_BobMinTroughVerticalOffset && m_BobVerticalOffset < currentBobOffsetValue.y && m_BobVerticalOffsetDecreasing) {
                m_SecondaryPositionSpring.AddForce(m_BobTroughForce);
            }
            m_BobVerticalOffsetDecreasing = currentBobOffsetValue.y < m_BobVerticalOffset;
            m_BobVerticalOffset = currentBobOffsetValue.y;
        }

        /// <summary>
        /// Adjusts the vertical offset by the given amount.
        /// </summary>
        /// <param name="amount">The amount to adjust the vertical offset height by.</param>
        private void AdjustVerticalOffset(float amount)
        {
            m_TargetVerticalOffsetAdjustment += amount;
            if (m_OffsetExtension != null) {
                m_OffsetExtension.Offset = m_CameraOffset + Vector3.up * m_TargetVerticalOffsetAdjustment;
            }
        }
    }
}

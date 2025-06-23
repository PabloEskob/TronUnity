// MovementConfig.cs

using UnityEngine;

namespace Config.Movement
{
    [CreateAssetMenu(menuName = "Config/Movement Config", fileName = "MovementConfig")]
    public sealed class MovementConfig : ScriptableObject
    {
        [Header("Speed Settings")] public float walkSpeed = 3.5f;
        public float runSpeed = 6.5f;
        public float sprintSpeed = 8.5f;
        public float crouchSpeed = 1.5f;

        [Header("Jump Settings")] public float jumpForce = 9f;
        public float sprintJumpForce = 12f;
        public float airControl = 0.3f;

        [Header("Dodge Settings")] public float dodgeSpeed = 7f;
        public float dodgeDuration = 0.55f;
        public AnimationCurve dodgeSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Rotation Settings")] public float walkRotationSpeed = 720f;
        public float runRotationSpeed = 540f;
        public float strafeRotationSpeed = 360f;

        [Header("Physics")] public float gravityMultiplier = 1.5f;
        public float groundCheckDistance = 0.4f;
    }
}
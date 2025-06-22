using UnityEngine;

namespace Config.Movement
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Character/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("Walk Settings")] public float walkSpeed = 3f;
        public float walkRotationSpeed = 10f;
        public AnimationCurve walkAccelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Run Settings")] public float runSpeed = 6f;
        public float runRotationSpeed = 15f;
        public float runStaminaDrain = 10f;

        [Header("Dodge Settings")] public float dodgeSpeed = 10f;
        public float dodgeDuration = 0.5f;
        public float dodgeCooldown = 1f;
        public AnimationCurve dodgeSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Jump Settings")] public float jumpForce = 8f;
        public float jumpCooldown = 0.1f;
        public int maxAirJumps = 1;

        [Header("Physics")] public float gravity = -19.62f; // 2x Unity's default
        public float groundCheckDistance = 0.1f;
        public LayerMask groundLayer = -1;
        public float maxFallSpeed = 20f;

        [Header("Advanced")] public float coyoteTime = 0.1f; // Time after leaving ground where jump is still possible
        public float jumpBufferTime = 0.1f; // Time before landing where jump input is buffered
    }
}
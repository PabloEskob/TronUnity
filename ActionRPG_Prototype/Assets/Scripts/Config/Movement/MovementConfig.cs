// MovementConfig.cs

using UnityEngine;

namespace Config.Movement
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Character/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("Walk Settings")] public float walkSpeed = 3f;
        public float walkRotationSpeed = 10f;

        [Header("Run Settings")] public float runSpeed = 6f;
        public float runRotationSpeed = 15f;

        [Header("Dodge Settings")] public float dodgeSpeed = 10f;
        public float dodgeDuration = 0.5f;
        public float dodgeCooldown = 1f;

        [Header("Physics")] public float gravity = -9.81f;
        public float groundCheckDistance = 0.1f;
        public LayerMask groundLayer;
    }
}
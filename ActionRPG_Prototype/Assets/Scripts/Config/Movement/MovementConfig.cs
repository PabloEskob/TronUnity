using UnityEngine;

namespace Config.Movement
{
    [CreateAssetMenu(menuName="Pavel/Movement Config", fileName="MovementConfig")]
    public sealed class MovementConfig : ScriptableObject
    {
        [Header("Speeds")]
        public float walkSpeed = 3.5f;
        public float runSpeed  = 6.5f;

        [Header("Rotation")]
        public float walkRotationSpeed = 12f;
        public float runRotationSpeed  = 18f;

        [Header("Jump / Dodge")]
        public float jumpForce      = 9f;
        public float dodgeSpeed     = 7f;
        public float dodgeDuration  = 0.55f;
        public AnimationCurve dodgeSpeedCurve = AnimationCurve.EaseInOut(0,1,1,0);
    }

}
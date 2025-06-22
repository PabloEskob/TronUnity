using UnityEngine;

namespace Config.Camera
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Camera/Camera Config")]
    public class CameraConfig : ScriptableObject
    {
        [Header("State Configurations")] public CameraStateConfig freeLookConfig;
        public CameraStateConfig lockOnConfig;
        public CameraStateConfig combatConfig;

        [Header("Collision")] public LayerMask collisionMask = -1;
        public float collisionOffset = 0.2f;
        public float collisionSmoothTime = 0.1f;

        [Header("General")] public float defaultFieldOfView = 60f;
        public float minPitch = -30f;
        public float maxPitch = 60f;
    }
}
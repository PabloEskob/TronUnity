using UnityEngine;

namespace Config.Camera
{
    [CreateAssetMenu(menuName = "Config/Camera Settings")]
    public class CameraConfig : ScriptableObject
    {
        [Header("Distance")] public float defaultDistance = 4.5f;
        public float minDistance = 2f;
        public float maxDistance = 10f;
        public float zoomSpeed = 5f;

        [Header("Rotation")] public float mouseSensitivity = 2f;
        public float gamepadSensitivity = 100f;
        public bool invertY = false;
        public float pitchMin = -30f;
        public float pitchMax = 70f;

        [Header("Smoothing")] public float rotationSmoothTime = 0.05f;
        public float zoomSmoothTime = 0.1f;

        [Header("Auto Features")] public bool enableAutoRotation = true;
        public float autoRotationDelay = 3f;
        public float autoRotationSpeed = 1f;

        [Header("Third Person Settings")] public Vector3 shoulderOffset = new(-0.25f, 0.9f, 0);
        public float cameraSide = 0.5f;
        public float verticalArmLength = 0.4f;

        [Header("Collision")] public LayerMask collisionMask = -1;
        public float cameraRadius = 0.2f;
    }
}
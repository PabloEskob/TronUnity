// CameraConfig.cs
using UnityEngine;

namespace Config.Camera
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Camera/Camera Config")]
    public class CameraConfig : ScriptableObject
    {
        [Header("Follow Settings")]
        public float followDistance = 5f;
        public float minDistance = 2f;
        public float maxDistance = 10f;
        public float followHeight = 2f;
        public float followDamping = 0.1f;
        
        [Header("Rotation Settings")]
        public float horizontalSensitivity = 2f;
        public float verticalSensitivity = 2f;
        public float minVerticalAngle = -30f;
        public float maxVerticalAngle = 70f;
        public float rotationDamping = 0.1f;
        
        [Header("Collision Settings")]
        public float collisionRadius = 0.3f;
        public LayerMask collisionLayers = -1;
        public float collisionDamping = 0.1f;
        public float minCollisionDistance = 1f;
        
        [Header("Zoom Settings")]
        public float zoomSpeed = 2f;
        public float zoomDamping = 0.1f;
        
        [Header("Combat Settings")]
        public float combatDistance = 7f;
        public float combatHeight = 2.5f;
        public float combatFOV = 50f;
        public float combatTransitionSpeed = 2f;
        
        [Header("Lock-On Settings")]
        public float lockOnDistance = 4f;
        public float lockOnHeight = 1.5f;
        public float lockOnRotationSpeed = 5f;
        public float lockOnFOV = 45f;
    }
}
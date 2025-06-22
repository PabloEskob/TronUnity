using UnityEngine;

namespace Config.Camera
{
    [CreateAssetMenu(fileName = "CameraStateConfig", menuName = "Camera/State Config")]
    public class CameraStateConfig : ScriptableObject
    {
        [Header("Base Settings")] public float fieldOfView = 60f;
        public float smoothTime = 0.2f;

        [Header("Free Look Settings")] public float defaultDistance = 5f;
        public float minDistance = 2f;
        public float maxDistance = 10f;
        public float defaultHeight = 2f;
        public float minHeight = 0.5f;
        public float maxHeight = 5f;
        public float horizontalSensitivity = 200f;
        public float verticalSensitivity = 100f;
        public float zoomSensitivity = 2f;
        public float targetHeightOffset = 1.5f;

        [Header("Lock On Settings")] public float distance = 7f;
        public float height = 3f;
        public float playerHeightOffset = 1f;
        public float lookHeightOffset = 1.5f;
        public float focusDistance = 3f;
        public float orbitSensitivity = 50f;
        public float rotationSpeed = 5f;

        [Header("Combat Settings")] public float combatDistance = 8f;
        public float combatHeight = 4f;
        public float combatFOV = 70f;
    }
}
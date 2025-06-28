using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.CameraLogic
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase _aimCamera;
        [SerializeField] private CinemachineVirtualCameraBase _freeCamera;

        public void Follow(GameObject target)
        {
            var playerAimController = target.GetComponentInChildren<SimplePlayerAimController>();
            _aimCamera.Follow = playerAimController.transform;
            _freeCamera.Follow = playerAimController.transform;
        }
    }
}
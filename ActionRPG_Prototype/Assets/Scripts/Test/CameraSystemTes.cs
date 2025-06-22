using Core.Camera;
using Core.Camera.States;
using Core.Services;
using UnityEngine;

namespace Test
{
    public class CameraSystemTest : MonoBehaviour
    {
        private CameraController _cameraController;
        private CameraStateManager _stateManager;

        void Start()
        {
            _cameraController = ServiceLocator.Instance.GetService<CameraController>();
            _stateManager = FindFirstObjectByType<CameraStateManager>();
        }

        void Update()
        {
            // Тест переключения состояний
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UnityEngine.Debug.Log("Switching to FreeLook");
                _stateManager.ChangeState<FreeLookCameraState>();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UnityEngine.Debug.Log("Switching to Combat");
                _stateManager.ChangeState<CombatCameraState>();
            }

            // Тест shake эффекта
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UnityEngine.Debug.Log("Camera Shake!");
                _cameraController.Shake(0.5f, 0.3f);
            }

            // Тест FOV эффекта
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UnityEngine.Debug.Log("FOV Change!");
                _cameraController.SetFOV(80f, 0.5f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                UnityEngine.Debug.Log("FOV Reset!");
                _cameraController.ResetFOV(0.5f);
            }
        }
    }
}
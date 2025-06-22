using Core.Input.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input.Providers
{
    public class CameraInputProvider : MonoBehaviour, ICameraInput, IInputProvider
    {
        private PlayerInputActions _inputActions;
        
        // ICameraInput implementation
        public Vector2 LookInput { get; private set; }
        public float ZoomInput { get; private set; }
        public bool IsLockOnPressed { get; private set; }
        public bool IsCameraModeTogglePressed { get; }
        public bool IsResetCameraPressed { get; private set; }
        
        public void Initialize(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
            SubscribeToEvents();
        }
        
        private void SubscribeToEvents()
        {
            if (_inputActions == null) return;
            
            _inputActions.Player.Look.performed += OnLook;
            _inputActions.Player.Look.canceled += OnLook;
            
            _inputActions.Player.CameraZoom.performed += OnZoom;
            _inputActions.Player.CameraZoom.canceled += OnZoom;
            
            _inputActions.Player.LockOn.started += _ => IsLockOnPressed = true;
            _inputActions.Player.LockOn.canceled += _ => IsLockOnPressed = false;
            
            _inputActions.Player.ResetCamera.started += _ => IsResetCameraPressed = true;
            _inputActions.Player.ResetCamera.canceled += _ => IsResetCameraPressed = false;
        }
        
        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }
        
        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<Vector2>().y;
        }
        
        private void OnDestroy()
        {
            if (_inputActions == null) return;
            
            _inputActions.Player.Look.performed -= OnLook;
            _inputActions.Player.Look.canceled -= OnLook;
            
            _inputActions.Player.CameraZoom.performed -= OnZoom;
            _inputActions.Player.CameraZoom.canceled -= OnZoom;
            
            _inputActions.Player.LockOn.started -= _ => IsLockOnPressed = true;
            _inputActions.Player.LockOn.canceled -= _ => IsLockOnPressed = false;
            
            _inputActions.Player.ResetCamera.started -= _ => IsResetCameraPressed = true;
            _inputActions.Player.ResetCamera.canceled -= _ => IsResetCameraPressed = false;
        }
    }
}
using Camera.Interface;
using Movement.Interface;
using UnityEngine;

namespace Movement
{
    public class PlayerInputHandler : MonoBehaviour, IMovementInput, ICameraInputProvider
    {
        private PlayerInputActions _inputActions;
        private Vector2 _movementInput;
        private bool _isRunning;
        private bool _isDodging;

        // Поля для камеры
        private Vector2 _lookInput;
        private float _zoomInput;
        private bool _isResetCameraPressed;
        private bool _isLockOnPressed;

        public Vector2 MovementVector => _movementInput;
        public bool IsRunning => _isRunning;
        public bool IsDodging => _isDodging;

        // Реализация интерфейса ICameraInputProvider
        public Vector2 LookInput => _lookInput;
        public float ZoomInput => _zoomInput;
        public bool IsResetCameraPressed => _isResetCameraPressed;
        public bool IsLockOnPressed => _isLockOnPressed;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            SetupInputCallbacks();
        }

        private void SetupInputCallbacks()
        {
            // Movement
            _inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => _movementInput = Vector2.zero;

            _inputActions.Player.Run.performed += ctx => _isRunning = true;
            _inputActions.Player.Run.canceled += ctx => _isRunning = false;

            _inputActions.Player.Dodge.performed += ctx => _isDodging = true;
            _inputActions.Player.Dodge.canceled += ctx => _isDodging = false;

            // Camera Look (например, мышь или стики)
            _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;

            // Zoom (например, колесо мыши)
            _inputActions.Player.CameraZoom.performed += ctx => _zoomInput = ctx.ReadValue<Vector2>().y;
            _inputActions.Player.CameraZoom.canceled += ctx => _zoomInput = 0f;

            // Reset Camera (кнопка)
            _inputActions.Player.ResetCamera.performed += ctx => _isResetCameraPressed = true;
            _inputActions.Player.ResetCamera.canceled += ctx => _isResetCameraPressed = false;

            // Lock On (кнопка)
            _inputActions.Player.LockOn.performed += ctx => _isLockOnPressed = true;
            _inputActions.Player.LockOn.canceled += ctx => _isLockOnPressed = false;
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();
    }
}

using Assets.Scripts.Movement.Interface;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class PlayerInputHandler : MonoBehaviour, IMovementInput
    {
        private PlayerInputActions _inputActions;
        private Vector2 _movementInput;
        private bool _isRunning;
        private bool _isDodging;

        public Vector2 MovementVector => _movementInput;
        public bool IsRunning => _isRunning;
        public bool IsDodging => _isDodging;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            SetupInputCallbacks();
        }
        
        private void SetupInputCallbacks()
        {
            _inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => _movementInput = Vector2.zero;

            _inputActions.Player.Run.performed += ctx => _isRunning = true;
            _inputActions.Player.Run.canceled += ctx => _isRunning = false;

            _inputActions.Player.Dodge.performed += ctx => _isDodging = true;
            _inputActions.Player.Dodge.canceled += ctx => _isDodging = false;
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();
    }
}
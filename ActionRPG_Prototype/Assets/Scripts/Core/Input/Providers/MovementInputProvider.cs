using Core.Input.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input.Providers
{
    public class MovementInputProvider : MonoBehaviour, IMovementInput, IInputProvider
    {
        private PlayerInputActions _inputActions;

        // IMovementInput implementation
        public Vector2 MovementVector { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsAttacking { get; }
        public bool IsDodging { get; private set; }

        public void Initialize(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (_inputActions == null) return;

            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMove;

            _inputActions.Player.Sprint.started += _ => IsRunning = true;
            _inputActions.Player.Sprint.canceled += _ => IsRunning = false;

            _inputActions.Player.Jump.started += _ => IsJumping = true;
            _inputActions.Player.Jump.canceled += _ => IsJumping = false;

            _inputActions.Player.Dodge.started += _ => IsDodging = true;
            _inputActions.Player.Dodge.canceled += _ => IsDodging = false;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MovementVector = context.ReadValue<Vector2>();
        }

        private void OnDestroy()
        {
            if (_inputActions == null) return;

            _inputActions.Player.Move.performed -= OnMove;
            _inputActions.Player.Move.canceled -= OnMove;

            _inputActions.Player.Sprint.started -= _ => IsRunning = true;
            _inputActions.Player.Sprint.canceled -= _ => IsRunning = false;

            _inputActions.Player.Jump.started -= _ => IsJumping = true;
            _inputActions.Player.Jump.canceled -= _ => IsJumping = false;

            _inputActions.Player.Dodge.started -= _ => IsDodging = true;
            _inputActions.Player.Dodge.canceled -= _ => IsDodging = false;
        }
    }
}
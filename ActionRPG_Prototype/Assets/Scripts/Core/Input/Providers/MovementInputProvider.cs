using Core.Input.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input.Providers
{
    public sealed class MovementInputProvider : InputProviderBase<IMovementInput>, IMovementInput
    {
        public Vector2 MovementVector { get; private set; }
        public bool IsRunning => input.Player.Sprint.IsPressed();
        public bool IsJumping => input.Player.Jump.WasPressedThisFrame();
        public bool IsDodging => input.Player.Dodge.IsPressed();
        public bool IsAttacking => input.Player.Attack.IsPressed();

        private void Update() => MovementVector = input.Player.Move.ReadValue<Vector2>();

        protected override void RegisterCallbacks()
        {
        }

        protected override void UnregisterCallbacks()
        {
        }
    }
}
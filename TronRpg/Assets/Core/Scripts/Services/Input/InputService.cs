using System;
using UnityEngine;

namespace Core.Scripts.Services.Input
{
    public class InputService : IInputService
    {
        private readonly GameInput _gameInput;
        private Vector2 _cachedMovement;
        private Vector2 _cachedLook;

        public InputService(GameInput gameInput)
        {
            _gameInput = gameInput ?? throw new ArgumentNullException(nameof(gameInput));
            _gameInput.Enable();
            SubscribeToEvents();
        }

        public Vector2 AxisMove => _cachedMovement;
        public Vector2 AxisLook => _cachedLook;
        public bool IsAttackButtonUp() => _gameInput.Player.Fire.WasReleasedThisFrame();

        private void SubscribeToEvents()
        {
            _gameInput.Player.Move.performed += ctx => _cachedMovement = ctx.ReadValue<Vector2>();
            _gameInput.Player.Move.canceled += _ => _cachedMovement = Vector2.zero;
            
            _gameInput.Player.Look.performed += ctx => _cachedLook = ctx.ReadValue<Vector2>();
            _gameInput.Player.Look.canceled += _ => _cachedLook = Vector2.zero;
        }
    }
}
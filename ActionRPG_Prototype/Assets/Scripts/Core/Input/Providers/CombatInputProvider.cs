using Core.Input.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input.Providers
{
    public sealed class CombatInputProvider : InputProviderBase<ICombatInput>, ICombatInput
    {
        private int _weaponDir;
        public bool IsAttacking => input.Player.Attack.IsPressed();
        public bool IsBlocking => false; // map if needed
        public bool IsSpecialAttack => false; // map
        public int WeaponSwitchDirection { get; private set; }

        protected override void RegisterCallbacks()
        {
            input.Player.Previous.performed += OnPrev;
            input.Player.Next.performed += OnNext;
            input.Player.Previous.canceled += OnStop;
            input.Player.Next.canceled += OnStop;
        }

        protected override void UnregisterCallbacks()
        {
            input.Player.Previous.performed -= OnPrev;
            input.Player.Next.performed -= OnNext;
            input.Player.Previous.canceled -= OnStop;
            input.Player.Next.canceled -= OnStop;
        }

        private void OnPrev(InputAction.CallbackContext _) => _weaponDir = -1;
        private void OnNext(InputAction.CallbackContext _) => _weaponDir = 1;
        private void OnStop(InputAction.CallbackContext _) => _weaponDir = 0;
        private void Update() => WeaponSwitchDirection = _weaponDir;
    }
}
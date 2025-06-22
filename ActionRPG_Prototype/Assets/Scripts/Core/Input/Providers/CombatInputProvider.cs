using Core.Input.Interfaces;
using UnityEngine;

namespace Core.Input.Providers
{
    public class CombatInputProvider : MonoBehaviour, ICombatInput
    {
        private PlayerInputActions _inputActions;
        private bool _isAttacking;
        private bool _isBlocking;
        private bool _isSpecialAttack;
        private int _weaponSwitchDirection;

        public bool IsAttacking => _isAttacking;
        public bool IsBlocking => _isBlocking;
        public bool IsSpecialAttack => _isSpecialAttack;
        public int WeaponSwitchDirection => _weaponSwitchDirection;

        public void Initialize(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _inputActions.Player.Attack.performed += _ => _isAttacking = true;
            _inputActions.Player.Attack.canceled += _ => _isAttacking = false;

            // Добавьте другие боевые действия по необходимости
            _inputActions.Player.Previous.performed += _ => _weaponSwitchDirection = -1;
            _inputActions.Player.Next.performed += _ => _weaponSwitchDirection = 1;
            _inputActions.Player.Previous.canceled += _ => _weaponSwitchDirection = 0;
            _inputActions.Player.Next.canceled += _ => _weaponSwitchDirection = 0;
        }
    }
}
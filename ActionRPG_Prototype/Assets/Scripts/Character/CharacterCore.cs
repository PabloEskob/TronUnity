// CharacterCore.cs - главный компонент персонажа

using CharacterMovement;
using Config.Character;
using UnityEngine;

namespace Character
{
    public class CharacterCore : MonoBehaviour
    {
        [Header("Core Components")] [SerializeField]
        private CharacterMovementController _movementController;

        [SerializeField] private CharacterAnimationController _animationController;
        [SerializeField] private CombatController _combatController;

        [Header("Configuration")] [SerializeField]
        private CharacterConfig _characterConfig;

        public CharacterMovementController Movement => _movementController;
        public CharacterAnimationController Animation => _animationController;
        public CombatController Combat => _combatController;

        private void Awake()
        {
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (_movementController == null)
                _movementController = GetComponent<CharacterMovementController>();

            if (_animationController == null)
                _animationController = GetComponentInChildren<CharacterAnimationController>();

            UnityEngine.Debug.Assert(_movementController != null, "Movement Controller is missing!");
            UnityEngine.Debug.Assert(_animationController != null, "Animation Controller is missing!");
        }
    }
}
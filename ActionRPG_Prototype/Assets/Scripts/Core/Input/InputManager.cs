// InputManager.cs

using UnityEngine;
using Core.Input.Interfaces;

namespace Core.Input
{
    [AddComponentMenu("Pavel/Input Manager")]
    [DisallowMultipleComponent]
    public sealed class InputManager : MonoBehaviour,
        IMovementInput,
        ICameraInput,
        ICombatInput
    {
        public PlayerInputActions Actions { get; private set; }

        // ───── internal providers ─────
        private Providers.MovementInputProvider _moveProv;
        private Providers.CameraInputProvider _camProv;
        private Providers.CombatInputProvider _combatProv;

        // ───── IMovementInput API ─────
        public Vector2 MovementVector => _moveProv.MovementVector;
        public bool IsRunning => _moveProv.IsRunning;
        public bool IsJumping => _moveProv.IsJumping;
        public bool IsDodging => _moveProv.IsDodging;
        public bool IsAttacking => _moveProv.IsAttacking;

        // ───── ICameraInput API ─────
        public Vector2 LookInput => _camProv.LookInput;
        public float ZoomInput => _camProv.ZoomInput;
        public bool IsResetCameraPressed => _camProv.IsResetCameraPressed;
        public bool IsLockOnPressed => _camProv.IsLockOnPressed;

        // ───── ICombatInput API ─────
        public bool IsBlocking => _combatProv.IsBlocking;
        public bool IsSpecialAttack => _combatProv.IsSpecialAttack;
        public int WeaponSwitchDirection => _combatProv.WeaponSwitchDirection;

        private void Awake()
        {
            Actions = new PlayerInputActions();
            Actions.Enable();

            _moveProv = gameObject.AddComponent<Providers.MovementInputProvider>();
            _camProv = gameObject.AddComponent<Providers.CameraInputProvider>();
            _combatProv = gameObject.AddComponent<Providers.CombatInputProvider>();

            foreach (var p in GetComponents<IInputProvider>())
                p.Initialize(Actions);
        }

        private void OnDisable() => Actions.Disable();
        private void OnDestroy() => Actions.Dispose();
    }
}
using Core.Input.Interfaces;
using UnityEngine;

namespace Core.Input.Providers
{
    public sealed class CameraInputProvider : InputProviderBase<ICameraInput>, ICameraInput
    {
        public Vector2 LookInput { get; private set; }
        public float ZoomInput { get; private set; }
        public bool IsResetCameraPressed => input.Player.ResetCamera.WasPressedThisFrame();
        public bool IsLockOnPressed => input.Player.LockOn.WasPressedThisFrame();

        private void Update()
        {
            LookInput = input.Player.Look.ReadValue<Vector2>();
            ZoomInput = input.Player.CameraZoom.ReadValue<Vector2>().y;
        }

        protected override void RegisterCallbacks()
        {
        }

        protected override void UnregisterCallbacks()
        {
        }
    }
}
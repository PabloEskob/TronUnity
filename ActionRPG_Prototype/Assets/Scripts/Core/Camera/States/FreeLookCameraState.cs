using Core.Camera.States;
using UnityEngine;

namespace Core.Camera.States
{
    public class FreeLookCameraState : CameraStateBase
    {
        private float _currentDistance;
        private float _currentHeight;
        private float _currentRotation;
        private Vector3 _smoothVelocity;

        public FreeLookCameraState(CameraStateManager stateManager, CameraController controller)
            : base(stateManager, controller)
        {
        }

        public override void EnterState()
        {
            _controller.SetConfig(Config.freeLookConfig);
            _currentDistance = Config.freeLookConfig.defaultDistance;
            _currentHeight = Config.freeLookConfig.defaultHeight;
        }

        public override void UpdateState()
        {
            if (Target == null) return;

            HandleInput();
            HandleCollision();
        }

        public override void LateUpdateState()
        {
            if (Target == null) return;

            UpdateCameraPosition();
            UpdateCameraRotation();
        }

        private void HandleInput()
        {
            var input = _controller.Input;

            // Horizontal rotation
            _currentRotation += input.LookInput.x * Config.freeLookConfig.horizontalSensitivity * Time.deltaTime;

            // Vertical adjustment
            _currentHeight += input.LookInput.y * Config.freeLookConfig.verticalSensitivity * Time.deltaTime;
            _currentHeight = Mathf.Clamp(_currentHeight, Config.freeLookConfig.minHeight, Config.freeLookConfig.maxHeight);

            // Distance adjustment
            _currentDistance -= input.ZoomInput * Config.freeLookConfig.zoomSensitivity;
            _currentDistance = Mathf.Clamp(_currentDistance, Config.freeLookConfig.minDistance, Config.freeLookConfig.maxDistance);
        }

        private void UpdateCameraPosition()
        {
            var targetPosition = Target.position + Vector3.up * Config.freeLookConfig.targetHeightOffset;

            // Calculate desired position
            var rotation = Quaternion.Euler(0, _currentRotation, 0);
            var offset = rotation * new Vector3(0, _currentHeight, -_currentDistance);
            var desiredPosition = targetPosition + offset;

            // Smooth movement
            _transform.position = Vector3.SmoothDamp(
                _transform.position,
                desiredPosition,
                ref _smoothVelocity,
                Config.freeLookConfig.smoothTime
            );
        }

        private void UpdateCameraRotation()
        {
            var lookTarget = Target.position + Vector3.up * Config.freeLookConfig.targetHeightOffset;
            _transform.LookAt(lookTarget);
        }

        private void HandleCollision()
        {
            if (Target == null) return;

            var targetPosition = Target.position + Vector3.up * Config.freeLookConfig.targetHeightOffset;
            var direction = (_transform.position - targetPosition).normalized;

            if (Physics.Raycast(targetPosition, direction, out var hit, _currentDistance, Config.collisionMask))
            {
                _currentDistance = hit.distance - Config.collisionOffset;
            }
        }
    }
}
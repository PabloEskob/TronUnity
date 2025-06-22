using UnityEngine;

namespace Core.Camera.States
{
    public class LockOnCameraState : CameraStateBase
    {
        private Vector3 _smoothVelocity;
        private float _currentAngle;

        public LockOnCameraState(CameraStateManager stateManager, CameraController controller)
            : base(stateManager, controller)
        {
        }

        public override void EnterState()
        {
            _controller.SetConfig(Config.lockOnConfig);
            CalculateInitialAngle();
        }

        public override void UpdateState()
        {
            if (Target == null || _stateManager.LockOnTarget == null)
            {
                _stateManager.ChangeState<FreeLookCameraState>();
                return;
            }

            HandleInput();
        }

        public override void LateUpdateState()
        {
            if (Target == null || _stateManager.LockOnTarget == null) return;

            UpdateCameraPosition();
            UpdateCameraRotation();
        }

        private void CalculateInitialAngle()
        {
            if (Target == null || _stateManager.LockOnTarget == null) return;

            var toTarget = _stateManager.LockOnTarget.position - Target.position;
            _currentAngle = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
        }

        private void HandleInput()
        {
            var input = _controller.Input;

            // Allow slight orbit around locked target
            _currentAngle += input.LookInput.x * Config.lockOnConfig.orbitSensitivity * Time.deltaTime;
        }

        private void UpdateCameraPosition()
        {
            var playerPos = Target.position + Vector3.up * Config.lockOnConfig.playerHeightOffset;
            var enemyPos = _stateManager.LockOnTarget.position + Vector3.up * Config.lockOnConfig.targetHeightOffset;

            // Calculate position between player and enemy
            var direction = (enemyPos - playerPos).normalized;
            var midPoint = playerPos + direction * Config.lockOnConfig.focusDistance;

            // Add orbit offset
            var orbitOffset = Quaternion.Euler(0, _currentAngle, 0) * Vector3.back * Config.lockOnConfig.distance;
            var desiredPosition = midPoint + orbitOffset + Vector3.up * Config.lockOnConfig.height;

            // Smooth movement
            _transform.position = Vector3.SmoothDamp(
                _transform.position,
                desiredPosition,
                ref _smoothVelocity,
                Config.lockOnConfig.smoothTime
            );
        }

        private void UpdateCameraRotation()
        {
            var lookPoint = (_stateManager.LockOnTarget.position + Target.position) * 0.5f;
            lookPoint.y += Config.lockOnConfig.lookHeightOffset;

            var rotation = Quaternion.LookRotation(lookPoint - _transform.position);
            _transform.rotation = Quaternion.Slerp(
                _transform.rotation,
                rotation,
                Config.lockOnConfig.rotationSpeed * Time.deltaTime
            );
        }
    }
}
using UnityEngine;

namespace Core.Camera.States
{
    public class CombatCameraState : CameraStateBase
    {
        private Vector3 _smoothVelocity;
        private float _currentAngle;

        public CombatCameraState(CameraStateManager stateManager, CameraController controller)
            : base(stateManager, controller)
        {
        }

        public override void EnterState()
        {
            _controller.SetConfig(Config.combatConfig);
            // Увеличиваем FOV для боя
            _controller.SetFOV(Config.combatConfig.combatFOV, 0.3f);
        }

        public override void UpdateState()
        {
            if (Target == null) return;
            HandleInput();
        }

        public override void LateUpdateState()
        {
            if (Target == null) return;
            UpdateCameraPosition();
            UpdateCameraRotation();
        }

        public override void ExitState()
        {
            _controller.ResetFOV(0.3f);
        }

        private void HandleInput()
        {
            var input = _controller.Input;
            _currentAngle += input.LookInput.x * Config.combatConfig.horizontalSensitivity * Time.deltaTime;
        }

        private void UpdateCameraPosition()
        {
            var targetPos = Target.position + Vector3.up * Config.combatConfig.playerHeightOffset;

            // Позиция камеры дальше и выше для обзора боя
            var rotation = Quaternion.Euler(0, _currentAngle, 0);
            var offset = rotation * new Vector3(0, Config.combatConfig.combatHeight, -Config.combatConfig.combatDistance);
            var desiredPosition = targetPos + offset;

            _transform.position = Vector3.SmoothDamp(
                _transform.position,
                desiredPosition,
                ref _smoothVelocity,
                Config.combatConfig.smoothTime
            );
        }

        private void UpdateCameraRotation()
        {
            var lookTarget = Target.position + Vector3.up * Config.combatConfig.lookHeightOffset;
            var rotation = Quaternion.LookRotation(lookTarget - _transform.position);
            _transform.rotation = Quaternion.Slerp(
                _transform.rotation,
                rotation,
                Config.combatConfig.rotationSpeed * Time.deltaTime
            );
        }
    }
}
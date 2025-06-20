using Assets.Scripts.Movement.Interface;
using CharacterMovement;
using Config.Movement;
using UnityEngine;

namespace Movement.State
{
    public class WalkState : MovementStateBase
    {
        private IMovementInput _input;
        private MovementConfig _config;

        public WalkState(CharacterMovementController controller, IMovementInput input,
            MovementConfig config) : base(controller)
        {
            _input = input;
            _config = config;
        }

        public override void Execute()
        {
            var inputVector = _input.MovementVector;
            if (inputVector.magnitude < 0.1f)
            {
                _controller.ChangeState<IdleState>();
                return;
            }

            if (_input.IsRunning)
            {
                _controller.ChangeState<RunState>();
                return;
            }

            if (_input.IsDodging && _controller.CanDodge)
            {
                _controller.ChangeState<DodgeState>();
                return;
            }

            var moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
            var cameraForward = Camera.main.transform.forward;
            var cameraRight = Camera.main.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            var worldDirection = cameraForward * inputVector.y + cameraRight * inputVector.x;

            _controller.Move(worldDirection, _config.walkSpeed);
            _controller.Rotate(worldDirection, _config.walkRotationSpeed);
        }
    }
}
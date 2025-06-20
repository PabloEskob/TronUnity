using Assets.Scripts.Movement.Interface;
using CharacterMovement;
using Config.Movement;
using UnityEngine;

namespace Movement.State
{
    public class DodgeState : MovementStateBase
    {
        private IMovementInput _input;
        private MovementConfig _config;
        private float _dodgeStartTime;
        private Vector3 _dodgeDirection;

        public DodgeState(CharacterMovementController controller, IMovementInput input,
            MovementConfig config) : base(controller)
        {
            _input = input;
            _config = config;
        }

        public override void Enter()
        {
            _dodgeStartTime = Time.time;
            _controller.SetDodgeTime();

            var inputVector = _input.MovementVector;
            if (inputVector.magnitude > 0.1f)
            {
                var cameraForward = Camera.main.transform.forward;
                var cameraRight = Camera.main.transform.right;

                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();

                _dodgeDirection = (cameraForward * inputVector.y +
                                   cameraRight * inputVector.x).normalized;
            }
            else
            {
                _dodgeDirection = _controller.transform.forward;
            }
        }

        public override void Execute()
        {
            if (Time.time - _dodgeStartTime > _config.dodgeDuration)
            {
                if (_input.MovementVector.magnitude > 0.1f)
                {
                    _controller.ChangeState<WalkState>();
                }
                else
                {
                    _controller.ChangeState<IdleState>();
                }

                return;
            }

            _controller.Move(_dodgeDirection, _config.dodgeSpeed);
            _controller.Rotate(_dodgeDirection, _config.runRotationSpeed * 2f);
        }
    }
}
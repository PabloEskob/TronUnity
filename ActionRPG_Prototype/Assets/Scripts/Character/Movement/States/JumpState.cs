using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public class JumpState : MovementStateBase
    {
        private bool _hasJumped;
        private float _jumpStartTime;

        public JumpState(MovementStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _hasJumped = false;
            _jumpStartTime = Time.time;
        }

        public override void Execute()
        {
            // Обработка ввода и логика перехода
            var moveDirection = _controller.GetMovementDirection();
            
            // Управление в воздухе
            if (moveDirection.magnitude > 0.1f)
            {
                var airSpeed = _controller.Config.walkSpeed * 0.7f;
                _controller.Move(moveDirection, airSpeed);
                _controller.Rotate(moveDirection, _controller.Config.walkRotationSpeed);
            }

            // Проверка перехода в состояние падения
            if (_controller.Physics.Velocity.y < 0)
            {
                _stateMachine.ChangeState<FallState>();
            }
        }

        public override void FixedExecute()
        {
            // Физика прыжка выполняется в FixedUpdate
            if (!_hasJumped)
            {
                _controller.Physics.Jump(_controller.Config.jumpForce);
                _hasJumped = true;
            }
        }

        public override bool CanTransitionTo<T>()
        {
            // Запрещаем переход в некоторые состояния во время прыжка
            if (typeof(T) == typeof(DodgeState))
            {
                // Можно dodgить только после определенного времени в прыжке
                return Time.time - _jumpStartTime > 0.2f;
            }
            
            return true;
        }
    }
}
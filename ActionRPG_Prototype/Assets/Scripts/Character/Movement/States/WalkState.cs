using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public class WalkState : MovementStateBase
    {
        private float _lastDodgeTime;
        private const float DODGE_COOLDOWN = 0.5f;

        public WalkState(MovementStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Execute()
        {
            var input = _controller.Input;
            var moveDirection = _controller.GetMovementDirection();

            // Проверка переходов
            if (moveDirection.magnitude < 0.1f)
            {
                _stateMachine.ChangeState<IdleState>();
                return;
            }

            if (input.IsRunning && CanRun())
            {
                _stateMachine.ChangeState<RunState>();
                return;
            }

            if (input.IsDodging && CanDodge())
            {
                _stateMachine.ChangeState<DodgeState>();
                return;
            }

            if (input.IsJumping && _controller.Physics.IsGrounded)
            {
                _stateMachine.ChangeState<JumpState>();
                return;
            }

            // Проверка падения
            if (!_controller.Physics.IsGrounded)
            {
                _stateMachine.ChangeState<FallState>();
                return;
            }
        }

        public override void FixedExecute()
        {
            // Физическое движение
            var moveDirection = _controller.GetMovementDirection();
            _controller.Move(moveDirection, _controller.Config.walkSpeed);
            _controller.Rotate(moveDirection, _controller.Config.walkRotationSpeed);
        }

        private bool CanRun()
        {
            // Проверяем наличие стамины
            var stats = _controller.GetComponent<Character.Stats.CharacterStats>();
            return stats.CurrentStamina > 0;
        }

        private bool CanDodge()
        {
            // Проверяем кулдаун dodge
            return Time.time - _lastDodgeTime > DODGE_COOLDOWN;
        }

        public override void Exit()
        {
            // Запоминаем время для кулдауна dodge
            if (_stateMachine.CurrentStateType == typeof(DodgeState))
            {
                _lastDodgeTime = Time.time;
            }
        }
    }
}
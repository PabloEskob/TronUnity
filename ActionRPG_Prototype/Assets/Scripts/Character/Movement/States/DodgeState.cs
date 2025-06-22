using Character.Movement.States.Base;
using Core.Events;
using UnityEngine;

namespace Character.Movement.States
{
    public class DodgeState : MovementStateBase
    {
        private float _dodgeStartTime;
        private Vector3 _dodgeDirection;
        private bool _canCancelDodge;

        public DodgeState(MovementStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _dodgeStartTime = Time.time;
            _canCancelDodge = false;
            CalculateDodgeDirection();
            GameEvents.OnPlayerDodged.Invoke();
            
            // Запускаем таймер для возможности отмены
            _stateMachine.StartCoroutine(EnableDodgeCancel());
        }

        private System.Collections.IEnumerator EnableDodgeCancel()
        {
            // Разрешаем отмену dodge после 60% анимации
            yield return new WaitForSeconds(_controller.Config.dodgeDuration * 0.6f);
            _canCancelDodge = true;
        }

        private void CalculateDodgeDirection()
        {
            var moveDirection = _controller.GetMovementDirection();
            
            if (moveDirection.magnitude > 0.1f)
            {
                _dodgeDirection = moveDirection.normalized;
            }
            else
            {
                _dodgeDirection = _transform.forward;
            }
        }

        public override void Execute()
        {
            var dodgeDuration = _controller.Config.dodgeDuration;
            var elapsedTime = Time.time - _dodgeStartTime;
            
            if (elapsedTime > dodgeDuration)
            {
                // Переход после завершения dodge
                TransitionToNextState();
                return;
            }

            // Проверяем возможность досрочного выхода
            if (_canCancelDodge && _controller.Input.IsAttacking)
            {
                _stateMachine.ChangeState<IdleState>(); // Или CombatState
                return;
            }
        }

        public override void FixedExecute()
        {
            // Физическое перемещение во время dodge
            var elapsedTime = Time.time - _dodgeStartTime;
            var normalizedTime = elapsedTime / _controller.Config.dodgeDuration;
            
            // Используем кривую скорости из конфига
            var speedMultiplier = _controller.Config.dodgeSpeedCurve.Evaluate(normalizedTime);
            var dodgeSpeed = _controller.Config.dodgeSpeed * speedMultiplier;
            
            _controller.Move(_dodgeDirection, dodgeSpeed);
            _controller.Rotate(_dodgeDirection, _controller.Config.runRotationSpeed * 2f);
        }

        public override bool CanTransitionTo<T>()
        {
            // Запрещаем выход из dodge до завершения или до разрешения отмены
            if (!_canCancelDodge)
            {
                var elapsedTime = Time.time - _dodgeStartTime;
                return elapsedTime >= _controller.Config.dodgeDuration;
            }
            
            // После разрешения отмены, разрешаем только определенные переходы
            if (typeof(T) == typeof(DodgeState))
            {
                // Нельзя dodge во время dodge
                return false;
            }
            
            return true;
        }

        private void TransitionToNextState()
        {
            var input = _controller.Input;
            
            if (input.MovementVector.magnitude > 0.1f)
            {
                if (input.IsRunning)
                {
                    _stateMachine.ChangeState<RunState>();
                }
                else
                {
                    _stateMachine.ChangeState<WalkState>();
                }
            }
            else
            {
                _stateMachine.ChangeState<IdleState>();
            }
        }
    }
}
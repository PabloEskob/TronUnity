// WalkState.cs
using Character.Movement.States.Base;
using UnityEngine;

namespace Character.Movement.States
{
    public sealed class WalkState : MovementStateBase
    {
        private float _lastDodge;
        private const float DodgeCooldown = 0.5f;

        public WalkState(MovementStateMachine m) : base(m) {}

        public override void Execute()
        {
            if (!IsGrounded) 
            { 
                _machine.ChangeState<FallState>(); 
                return; 
            }

            var inputDir = _controller.GetMovementDirection();
            
            if (inputDir.sqrMagnitude < 0.1f) 
            { 
                _machine.ChangeState<IdleState>(); 
                return; 
            }
            
            // Обновляем режим спринта
            _controller.SetSprinting(_controller.Input.IsRunning);
            
            if (_controller.Input.IsRunning && HasStamina()) 
            { 
                _machine.ChangeState<RunState>(); 
                return; 
            }
            
            if (_controller.Input.IsDodging && CanDodge()) 
            { 
                _machine.ChangeState<DodgeState>(); 
                return; 
            }
            
            if (_controller.Input.IsJumping) 
            { 
                _machine.ChangeState<JumpState>(); 
                return; 
            }

            // Обновляем режим стрейфа (например, при прицеливании)
           // _controller.SetStrafeMode(_controller.Input.IsBlocking);
        }

        public override void FixedExecute()
        {
            var inputDir = _controller.GetMovementDirection();
            _controller.Move(inputDir, _controller.Config.walkSpeed);
            _controller.Rotate(inputDir, _controller.Config.walkRotationSpeed);
        }

        public override void Exit()
        {
            if (_machine.CurrentStateType == typeof(DodgeState))
                _lastDodge = Time.time;
        }

        private bool HasStamina() => 
            _controller.GetComponent<Stats.CharacterStats>()?.CurrentStamina > 0;
            
        private bool CanDodge() => 
            Time.time - _lastDodge > DodgeCooldown;
    }
}
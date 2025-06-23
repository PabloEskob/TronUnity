using Character.Movement.States.Base;
using Core.Events.Messages;
using UnityEngine;

namespace Character.Movement.States
{
    public sealed class DodgeState : MovementStateBase
    {
        readonly float _duration;
        readonly float _cancelWindow; // когда можно отменить в атаку

        float   _start;
        Vector3 _dir;

        public DodgeState(MovementStateMachine m) : base(m)
        {
            _duration      = _controller.Config.dodgeDuration;
            _cancelWindow  = 0.6f;
        }

        public override void Enter()
        {
            _start = Time.time;
            _dir   = InputDir.sqrMagnitude > .1f ? InputDir.normalized : _tf.forward;
            _bus.Publish(new PlayerDodged());
        }

        public override void Execute()
        {
            float t = (Time.time - _start) / _duration;
            if (t >= 1f)                         { Transition(); return; }
            if (t >= _cancelWindow && _controller.Input.IsAttacking)
                _machine.ChangeState<IdleState>();
        }

        public override void FixedExecute()
        {
            float t     = (Time.time - _start) / _duration;
            float speed = _controller.Config.dodgeSpeed
                          * _controller.Config.dodgeSpeedCurve.Evaluate(t);

            _controller.Move  (_dir, speed);
            _controller.Rotate(_dir, _controller.Config.runRotationSpeed * 2f);
        }

        public override bool CanTransitionTo<T>() => typeof(T) != typeof(DodgeState);

        void Transition()
        {
            if (InputDir.sqrMagnitude > .1f)
                _machine.ChangeState(_controller.Input.IsRunning
                    ? typeof(RunState) : typeof(WalkState));
            else
                _machine.ChangeState<IdleState>();
        }
    }
}
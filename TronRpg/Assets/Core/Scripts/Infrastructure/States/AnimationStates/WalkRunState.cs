using Core.Scripts.Character.Enemy;
using UnityEngine;

namespace Core.Scripts.Infrastructure.States.AnimationStates
{
    /// <summary>
    /// Состояние Walk/Run: проигрывает WalkRunMixer с blending по скорости.
    /// </summary>
    public class WalkRunState : IAnimationState
    {
        public void Enter(BaseEnemyAnimator animator)
        {
            var state = animator.PlayAnimation(animator.WalkRunMixerTransition);
            if (state != null)
            {
                var speed = animator.MovementProvider.Velocity.magnitude;
                var normalized = Mathf.InverseLerp(0f, animator.MovementProvider.MaxSpeed, speed);
                animator.SpeedParam.TargetValue = normalized;
            }
        }
    }
}
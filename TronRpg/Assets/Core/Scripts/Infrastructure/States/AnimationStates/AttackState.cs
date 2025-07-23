using Core.Scripts.Character.Enemy;
using UnityEngine;

namespace Core.Scripts.Infrastructure.States.AnimationStates
{
    /// <summary>
    /// Состояние Attack: проигрывает атакующую анимацию.
    /// </summary>
    public class AttackState : IAnimationState
    {
        public void Enter(BaseEnemyAnimator animator)
        {
            var state = animator.PlayAnimation(animator.AttackTransition);

            if (state != null)
            {
                state.Events(this).SetCallback(animator.AttackHitEventName, animator.RaiseAttackPerformed);
                state.Events(this).OnEnd = () =>
                    animator.RaiseStateEnded(BaseEnemyAnimator.EnemyState.Attack);
            }
        }
    }
}
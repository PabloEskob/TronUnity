using Core.Scripts.Character.Enemy;

namespace Core.Scripts.Infrastructure.States.AnimationStates
{
    /// <summary>
    /// Состояние Attack: проигрывает атакующую анимацию.
    /// </summary>
    public class AttackState : IAnimationState
    {
        public void Enter(BaseEnemyAnimator animator)
        {
            animator.PlayAnimation(animator.AttackTransition);
        }
    }
}
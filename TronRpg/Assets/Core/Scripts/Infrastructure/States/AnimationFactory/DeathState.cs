using Core.Scripts.Character.Enemy;

namespace Core.Scripts.Infrastructure.States.AnimationFactory
{
    /// <summary>
    /// Состояние Death: проигрывает анимацию смерти.
    /// </summary>
    public class DeathState : IAnimationState
    {
        public void Enter(BaseEnemyAnimator animator)
        {
            animator.PlayAnimation(animator.DeathTransition);
        }
    }
}
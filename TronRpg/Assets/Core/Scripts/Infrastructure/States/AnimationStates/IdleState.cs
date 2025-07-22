using Core.Scripts.Character.Enemy;

namespace Core.Scripts.Infrastructure.States.AnimationStates
{
    /// <summary>
    /// Состояние Idle: проигрывает случайную анимацию из библиотеки.
    /// </summary>
    public class IdleState : IAnimationState
    {
        public void Enter(BaseEnemyAnimator animator)
        {
            animator.PlayRandomIdle();
        }
    }
}
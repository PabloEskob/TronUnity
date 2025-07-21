using Animancer;

namespace Core.Scripts.Character.Enemy
{
    public interface IEnemyAnimator
    {
        void UpdateAnimationState(BaseEnemyAnimator.EnemyState newState);
        AnimancerState PlayAnimation(ITransition transition);

        BaseEnemyAnimator.EnemyState CurrentState { get; set; }
    }
}
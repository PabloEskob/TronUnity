using Animancer;

namespace Core.Scripts.Character.Enemy
{
    public interface IEnemyAnimator
    {
        void UpdateAnimationState(BaseEnemyAnimator.EnemyState newState);
        AnimancerState PlayAnimation(TransitionAsset transition);
    }
}
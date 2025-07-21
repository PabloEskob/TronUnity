using Core.Scripts.Character.Enemy;

namespace Core.Scripts.Infrastructure.States.AnimationFactory
{
    /// <summary>
    /// Интерфейс для состояния анимации, реализует паттерн State.
    /// </summary>
    public interface IAnimationState
    {
        void Enter(BaseEnemyAnimator animator);
    }
}
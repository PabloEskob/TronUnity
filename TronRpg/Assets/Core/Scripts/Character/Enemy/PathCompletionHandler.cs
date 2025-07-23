namespace Core.Scripts.Character.Enemy
{
    public class PathCompletionHandler : BaseMovementHandler
    {
        protected override void SubscribeToEvents()
        {
            MovementProvider.OnPathCompleted += HandlePathCompleted;
        }

        protected override void UnsubscribeFromEvents()
        {
            MovementProvider.OnPathCompleted -= HandlePathCompleted;
        }

        private void HandlePathCompleted()
        {
            if (Animator.CurrentState == BaseEnemyAnimator.EnemyState.Death) return;

            if (Animator.CurrentState == BaseEnemyAnimator.EnemyState.Walk ||
                Animator.CurrentState == BaseEnemyAnimator.EnemyState.Run)
            {
                Animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Idle);
            }
        }
    }
}
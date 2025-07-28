using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class AgentAnimationController : BaseMovementHandler
    {
        private float _cachedMaxSpeed;

        protected override void Awake()
        {
            base.Awake();
            if (!enabled) return;
            _cachedMaxSpeed = MovementProvider.MaxSpeed;
        }

        protected override void SubscribeToEvents()
        {
            MovementProvider.OnVelocityChanged += HandleVelocityChanged;
        }

        protected override void UnsubscribeFromEvents()
        {
            MovementProvider.OnVelocityChanged -= HandleVelocityChanged;
        }

        private void HandleVelocityChanged(Vector3 newVelocity)
        {
            var speed = newVelocity.magnitude;
            var normalizedSpeed = Mathf.InverseLerp(0f, _cachedMaxSpeed, speed);
            Animator.SetSpeedParam(normalizedSpeed);
        }
    }
}
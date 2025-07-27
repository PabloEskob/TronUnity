using Animancer;
using UnityEngine;

namespace Core.Scripts.Character.Animator
{
    public class HeroAnimator : MonoBehaviour
    {
        [SerializeField] private TransitionAsset AnimationMixerIdleRun;
        [SerializeField] private StringAsset ParameterNameMixerIdleRun;
        [Header("External")]
        [SerializeField] private ECM2.Character Controller;
        [SerializeField] private AnimancerComponent Animancer;

        private readonly float _actionFadeOutDuration = 0.25f;
        private SmoothedFloatParameter _speedParam;
        private AnimancerLayer _baseLayer;
        private AnimancerLayer _actionLayer;
        public bool IsAttacking { get; private set; }

        private void Awake()
        {
            _baseLayer = Animancer.Layers[0];
            _actionLayer = Animancer.Layers[1];
            _speedParam = new SmoothedFloatParameter(Animancer, ParameterNameMixerIdleRun, 0.05f);
            _baseLayer.Play(AnimationMixerIdleRun);
        }

        private void Update()
        {
            var horizontalVel = Vector3.ProjectOnPlane(Controller.velocity, Controller.GetGravityDirection());
            var speed = horizontalVel.magnitude;
            var normalized = Mathf.InverseLerp(0f, Controller.maxWalkSpeed, speed);
            _speedParam.TargetValue = normalized;
        }

        public void PlayHit(ClipTransition hit)
        {
            _actionLayer.Stop();
            _actionLayer.Play(hit).Events(this).OnEnd = ReturnToPrevious;
        }
        
        public void PlayDead(TransitionAsset deathTransition)
        {
            Animancer.Animator.applyRootMotion = true;
            _actionLayer.Play(deathTransition);
        }

        public void PlayAttack(TransitionAsset attackTransition)
        {
            IsAttacking = true;
            var state = _actionLayer.Play(attackTransition);
            state.Time = 0;
            state.Events(this).OnEnd = ReturnToPrevious;
        }

        private void ReturnToPrevious()
        {
            _actionLayer.StartFade(0, _actionFadeOutDuration);
            IsAttacking = false;
        }
    }
}
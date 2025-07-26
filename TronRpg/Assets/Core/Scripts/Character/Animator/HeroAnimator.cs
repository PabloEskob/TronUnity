using Animancer;
using UnityEngine;

namespace Core.Scripts.Character.Animator
{
    public class HeroAnimator : MonoBehaviour
    {
        [SerializeField] private TransitionAsset AnimationMixerIdleRun;

        [SerializeField] private ClipTransition Hit;

        [SerializeField] private StringAsset ParameterNameMixerIdleRun;

        [Header("External")] [SerializeField] private ECM2.Character Controller;
        [SerializeField] private AnimancerComponent Animancer;

        private SmoothedFloatParameter _speedParam;
        private AnimancerLayer _baseLayer;
        private AnimancerLayer _actionLayer;
        private float _actionFadeOutDuration = 0.25f;

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

        public void PlayHit()
        {
            Animancer.Animator.applyRootMotion = true;
            _actionLayer.Play(Hit).Events(this).OnEnd = ReturnToPrevious;
        }

        public void PlayTransition(TransitionAsset transition, int layer = 0)
        {
            var state = Animancer.Layers[layer].Play(transition);
        }

        private void ReturnToPrevious()
        {
            _actionLayer.StartFade(0, _actionFadeOutDuration);
            Animancer.Animator.applyRootMotion = false;
        }
    }
}
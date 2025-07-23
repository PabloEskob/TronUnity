using Animancer;
using UnityEngine;

namespace Core.Scripts.Character.Animator
{
    public class HeroAnimator : MonoBehaviour
    {
        [SerializeField] 
        private TransitionAsset AnimationMixerIdleRun;
        
        [SerializeField] 
        private TransitionAsset Hit;

        [SerializeField] 
        private StringAsset ParameterNameMixerIdleRun;

        [Header("External")]
        [SerializeField] private ECM2.Character Controller;
        [SerializeField] private AnimancerComponent Animancer;

        private SmoothedFloatParameter _speedParam;

        private void Awake()
        {
            _speedParam = new SmoothedFloatParameter(Animancer, ParameterNameMixerIdleRun, 0.05f);
            Animancer.Play(AnimationMixerIdleRun);
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
            Animancer.Play(Hit);
        }
    }
}
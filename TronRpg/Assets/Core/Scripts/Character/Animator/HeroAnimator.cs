using Animancer;
using UnityEngine;

namespace Core.Scripts.Character.Animator
{
    public class HeroAnimator : MonoBehaviour
    {
        [SerializeField] 
        private TransitionAsset AnimationMixerIdleRun;

        [SerializeField] 
        private StringAsset ParameterNameMixerIdleRun;

        [Header("External")]
        [SerializeField] private ECM2.Character Controller;
        [SerializeField] private AnimancerComponent Animancer;

        private Parameter<float> _speedParam;

        private void Awake()
        {
            _speedParam = Animancer.Parameters.GetOrCreate<float>(ParameterNameMixerIdleRun);
            Animancer.Play(AnimationMixerIdleRun);
        }

        private void Update()
        {
            var horizontalVel = Vector3.ProjectOnPlane(Controller.velocity, Controller.GetGravityDirection());
            var speed = horizontalVel.magnitude;
            var normalized = Mathf.InverseLerp(0f, Controller.maxWalkSpeed, speed);
            _speedParam.Value = Mathf.Clamp01(normalized);
        }
    }
}
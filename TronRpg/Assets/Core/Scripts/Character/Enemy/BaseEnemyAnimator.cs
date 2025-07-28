using System;
using Animancer;
using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public abstract class BaseEnemyAnimator : MonoBehaviour
    {
        [SerializeField] protected TransitionAsset AnimationMixerIdleRun;
        [SerializeField] protected StringAsset ParameterNameMixerIdleRun;

        [Header("External")] [SerializeField] protected MonoBehaviour Controller;
        [SerializeField] protected AnimancerComponent Animancer;
        
        protected bool isDead;

        private SmoothedFloatParameter _speedParam;
        private AnimancerLayer _baseLayer;
        private AnimancerLayer _actionLayer;

        protected virtual void Awake()
        {
            _baseLayer = Animancer.Layers[0];
            _actionLayer = Animancer.Layers[1];
            _speedParam = new SmoothedFloatParameter(Animancer, ParameterNameMixerIdleRun, 0.05f);
            _baseLayer.Play(AnimationMixerIdleRun);
            InitializeSpecific();
        }

        protected abstract void InitializeSpecific();

        public void SetSpeedParam(float normalizedSpeed)
        {
            if (isDead) return;
            _speedParam.TargetValue = normalizedSpeed;
        }

        public void PlayHit(ClipTransition hit)
        {
            if (isDead) return;
            var currentState = _actionLayer.CurrentState;
            if (currentState is { IsPlaying: true } && currentState.Events(this).OnEnd != null)
            {
                currentState.Events(this).OnEnd.Invoke();
                currentState.Events(this).OnEnd = null; 
            }
            
            _actionLayer.Play(hit).Events(this).OnEnd = ReturnToPrevious;
        }

        public void PlayDead(ClipTransition deathTransition)
        {
            if (isDead) return;
            isDead = true;
            _baseLayer.Stop();
            _baseLayer.SetWeight(0f);
            Animancer.Animator.applyRootMotion = true;
            _actionLayer.Play(deathTransition);
        }

        public void PlayAttack(TransitionAsset attackTransition, StringAsset nameAsset, Action callback, Action ended)
        {
            if (isDead) return;
            var state = _actionLayer.Play(attackTransition);
            state.Time = 0;
            state.Events(this).SetCallbacks(nameAsset, callback);
            state.Events(this).OnEnd = () => 
            {
                ended?.Invoke();
                ReturnToPrevious();
            };
        }

        private void ReturnToPrevious()
        {
            _actionLayer.StartFade(0);
        }
    }
}
using System;
using Animancer;
using Core.Scripts.Character.Animator;
using Core.Scripts.Data;
using Core.Scripts.Services.Input;
using Core.Scripts.Services.PersistentProgress;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Character.Hero
{
    public class HeroAttack : MonoBehaviour, ISavedProgressReader
    {
        [SerializeField] private TransitionAsset AttackTransition;

        public HeroAnimator Animator;
        public ECM2.Character Character;

        private IInputService _inputService;
        private Collider[] _hits = new Collider[3];
        private float _radius;
        private Stats _heroStats;
        private static int _layerMask;

        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            _layerMask = 1 << LayerMask.NameToLayer("Hittable");
        }

        private void Update()
        {
            if (_inputService.IsAttackButtonUp() && !Animator.IsAttacking)
            {
                Animator.PlayAttack(AttackTransition);
            }
        }

        public void OnAttack()
        {
        }

        private void Hit() => 
            Physics.OverlapSphereNonAlloc(StartPoint() + transform.forward, _heroStats.DamageRadius, _hits, _layerMask);

        public void LoadProgress(PlayerProgress playerProgress)
        {
            _heroStats = playerProgress.HeroStats;
        }

        private Vector3 StartPoint() => new Vector3(transform.position.x, (Character.height / 2) / 2, transform.position.z);
    }
}
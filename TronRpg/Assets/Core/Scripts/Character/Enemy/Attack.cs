using System.Linq;
using Animancer;
using Core.Scripts.Character.Hero;
using Core.Scripts.Character.Interface;
using Core.Scripts.Helpers.Phisics;
using Core.Scripts.Infrastructure.States.Factory;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Character.Enemy
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] FollowerEntityAdapter _follower;
        [SerializeField] private TransitionAsset AttackTransition;
        [SerializeField] private StringAsset AttackHitName;

        public BaseEnemyAnimator Animator;
        public float AttackCooldown = 3f;
        public float Cleavage = 0.5f;
        public float EffectiveDistance = 0.5f;
        public float Damage = 10f;

        private readonly Collider[] _hits = new Collider[1];
        [Inject] private IGameFactory _gameFactory;
        private Transform _heroTransform;
        private float _attackCooldown;
        private bool _isAttacking;
        private int _layerMask;
        private bool _attackIsActive;


        private void Awake()
        {
            _layerMask = 1 << LayerMask.NameToLayer("Player");
            _gameFactory.HeroCreated += OnHeroCreated;
        }

        private void Update()
        {
            UpdateCooldown();

            if (CanAttack())
                StartAttack();
        }

        public void EnableAttack()
        {
            _attackIsActive = true;
        }

        public void DisableAttack()
        {
            _attackIsActive = false;
        }

        private void UpdateCooldown()
        {
            if (!CooldownIsUp())
                _attackCooldown -= Time.deltaTime;
        }

        private void OnAttack()
        {
            if (Hit(out Collider hit))
            {
                hit.transform.GetComponent<IHealth>().TakeDamage(Damage);
            }
        }

        private bool Hit(out Collider hit)
        {
            var hitCount = Physics.OverlapSphereNonAlloc(StartPoint(), Cleavage, _hits, _layerMask);
            hit = _hits.FirstOrDefault();
            return hitCount > 0;
        }

        private Vector3 StartPoint()
        {
            return new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) +
                   transform.forward * EffectiveDistance;
        }

        private void OnAttackEnded()
        {
            _attackCooldown = AttackCooldown;
            _isAttacking = false;
            _follower?.ResumeMovement();
        }

        private bool CanAttack() =>
            _attackIsActive && !_isAttacking && CooldownIsUp();

        private bool CooldownIsUp()
        {
            return _attackCooldown <= 0;
        }

        private void StartAttack()
        {
            transform.LookAt(_heroTransform);
            _follower?.StopMovement();
            Animator.PlayAttack(AttackTransition, AttackHitName, OnAttack, OnAttackEnded);
            _isAttacking = true;
        }

        private void OnHeroCreated() =>
            _heroTransform = _gameFactory.HeroGameObject.transform;
    }
}
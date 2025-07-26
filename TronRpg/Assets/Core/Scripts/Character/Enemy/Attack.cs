using System.Linq;
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
            Animator.OnAnimationStateEnded += HandleAnimationEnded;
            Animator.OnAttack += OnAttack;
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
                PhysicsDebug.DrawDebug(StartPoint(), Cleavage, 3);
                hit.transform.GetComponent<HeroHealth>().TakeDamage(Damage);
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
            Animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Idle);
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
            Animator.UpdateAnimationState(BaseEnemyAnimator.EnemyState.Attack);
            _isAttacking = true;
        }

        private void OnHeroCreated() =>
            _heroTransform = _gameFactory.HeroGameObject.transform;

        private void HandleAnimationEnded(BaseEnemyAnimator.EnemyState state)
        {
            if (state == BaseEnemyAnimator.EnemyState.Attack)
            {
                OnAttackEnded();
            }
        }

        private void OnDestroy()
        {
            Animator.OnAnimationStateEnded -= HandleAnimationEnded;
            Animator.OnAttack -= OnAttack;
        }
    }
}
using Core.Scripts.Infrastructure.States.Factory;
using Pathfinding;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Character.Enemy
{
    public class AgentMoveToPlayer : Follow
    {
        [SerializeField] private float _minimalDistance = 2f;
        [SerializeField] private FollowerEntity _agent;
        
        private Transform _heroTransform;

        private void Start()
        {
            if (_gameFactory.HeroGameObject != null)
                InitializeHeroTransform();
            else
                _gameFactory.HeroCreated += HeroCreated;
        }

        private void Update()
        {
            if (!_isPursuing || !_heroTransform || !HeroNotReached()) return;
            _agent.destination = _heroTransform.position;
        }
        
        
        private bool HeroNotReached() =>
            Vector3.Distance(_agent.transform.position, _heroTransform.position) >= _minimalDistance;

        private void HeroCreated() =>
            InitializeHeroTransform();

        private void InitializeHeroTransform()
        {
            if (!_gameFactory.HeroGameObject)
            {
                Debug.LogWarning("HeroGameObject is null!");
                return;
            }

            _heroTransform = _gameFactory.HeroGameObject.transform;
        }

        private void OnDestroy() =>
            _gameFactory.HeroCreated -= HeroCreated;
    }
}
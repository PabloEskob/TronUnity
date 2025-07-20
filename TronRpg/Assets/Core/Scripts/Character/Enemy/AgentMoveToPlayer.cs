using Core.Scripts.Infrastructure.States.Factory;
using Pathfinding;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Character.Enemy
{
    public class AgentMoveToPlayer : MonoBehaviour
    {
        private const float MinimalDistance = 2;

        [SerializeField] private FollowerEntity _agent;

        [Inject]
        private IGameFactory _gameFactory;

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
            if (_heroTransform && HeroNotReached()) 
                _agent.destination = _heroTransform.position;
        }

        private bool HeroNotReached() => 
            Vector3.Distance(_agent.transform.position, _heroTransform.position) >= MinimalDistance;

        private void HeroCreated(GameObject obj) =>
            InitializeHeroTransform();

        private void InitializeHeroTransform() =>
            _heroTransform = _gameFactory.HeroGameObject.transform;
    }
}
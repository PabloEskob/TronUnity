using Core.Scripts.Infrastructure.States.Factory;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Character.Enemy
{
    public class RotateToHero : Follow
    {
        public float Speed;

        private Transform _heroTransform;
        private Vector3 _positionToLook;

        [Inject] private IGameFactory _gameFactory;

        private void Start()
        {
            if (HeroExists())
                InitializeHeroTransform();
            else
                _gameFactory.HeroCreated += InitializeHeroTransform;
        }

        private void Update()
        {
            if (Initialized())
                RotateTowardsHero();
        }

        private void RotateTowardsHero()
        {
            UpdatePositionToLookAt();

            transform.rotation = SmoothedRotation(_heroTransform.rotation, _positionToLook);
        }

        private void UpdatePositionToLookAt()
        {
            Vector3 positionDiff = _heroTransform.position - transform.position;
            _positionToLook = new Vector3(positionDiff.x, transform.position.y, positionDiff.z);
        }

        private Quaternion SmoothedRotation(Quaternion rotation, Vector3 positionToLook) =>
            Quaternion.Lerp(rotation, TargetRotation(positionToLook), SpeedFactor());

        private float SpeedFactor() =>
            Speed * Time.deltaTime;

        private Quaternion TargetRotation(Vector3 positionToLook) =>
            Quaternion.LookRotation(positionToLook);

        private bool Initialized()
        {
            return _heroTransform;
        }

        private void InitializeHeroTransform() =>
            _heroTransform = _gameFactory.HeroGameObject.transform;

        private bool HeroExists() =>
            _gameFactory.HeroGameObject;
    }
}
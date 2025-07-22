using UnityEngine;
using Pathfinding;
using System;

namespace Core.Scripts.Character.Enemy
{
    [RequireComponent(typeof(FollowerEntity))]
    public class FollowerEntityAdapter : MonoBehaviour, IMovementProvider
    {
        [SerializeField] private FollowerEntity _follower;

        public Vector3 Velocity => _follower.velocity;
        public float MaxSpeed => _follower.maxSpeed;
        public bool ReachedEndOfPath => _follower.reachedEndOfPath;

        public event Action OnPathCompleted;
        public event Action<Vector3> OnVelocityChanged;

        private Vector3 _lastVelocity;
        private bool _lastReached;

        private void Awake()
        {
            if (!_follower)
            {
                _follower = GetComponent<FollowerEntity>(); // Автоматизация для удобства
                if (!_follower)
                {
                    Debug.LogError("FollowerEntity not found!");
                    enabled = false;
                    return;
                }
            }
        }

        private void Update()
        {
            // Триггер OnPathCompleted только при изменении
            if (_follower.reachedEndOfPath && !_lastReached)
            {
                OnPathCompleted?.Invoke();
                _lastReached = true;
            }
            else if (!_follower.reachedEndOfPath)
            {
                _lastReached = false;
            }

            // Триггер OnVelocityChanged только при значимом изменении
            if (_follower.velocity != _lastVelocity)
            {
                OnVelocityChanged?.Invoke(_follower.velocity);
                _lastVelocity = _follower.velocity;
            }
        }

        public void StopMovement()
        {
            _follower.isStopped = true;
            OnPathCompleted?.Invoke();
        }

        public void ResumeMovement(float originalMaxSpeed = -1f)
        {
            _follower.isStopped = false;
            if (originalMaxSpeed > 0)
                _follower.maxSpeed = originalMaxSpeed;
        }
    }
}
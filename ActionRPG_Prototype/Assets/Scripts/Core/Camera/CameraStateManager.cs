using System;
using System.Collections.Generic;
using Config.Camera;
using Core.Camera.States;
using Core.Events;
using UnityEngine;

namespace Core.Camera
{
    public class CameraStateManager : MonoBehaviour
    {
        [SerializeField] private Transform _cameraRig;
        [SerializeField] private CameraConfig _defaultConfig;
        
        private Dictionary<Type, ICameraState> _states;
        private ICameraState _currentState;
        private CameraController _controller;
        
        public ICameraState CurrentState => _currentState;
        public Transform Target { get; set; }
        public Transform LockOnTarget { get; set; }
        
        private void Awake()
        {
            _controller = GetComponent<CameraController>();
            InitializeStates();
        }

        private void InitializeStates()
        {
            _states = new Dictionary<Type, ICameraState>
            {
                { typeof(FreeLookCameraState), new FreeLookCameraState(this, _controller) },
                { typeof(LockOnCameraState), new LockOnCameraState(this, _controller) },
                { typeof(CombatCameraState), new CombatCameraState(this, _controller) }
            };
        }

        private void Start()
        {
            ChangeState<FreeLookCameraState>();
        }

        private void Update()
        {
            _currentState?.UpdateState();
        }

        private void LateUpdate()
        {
            _currentState?.LateUpdateState();
        }

        public void ChangeState<T>() where T : ICameraState
        {
            var targetType = typeof(T);
            
            if (_currentState != null && _currentState.GetType() == targetType)
                return;

            if (!_states.TryGetValue(targetType, out var newState))
            {
                UnityEngine.Debug.LogError($"Camera state {targetType.Name} not found!");
                return;
            }

            _currentState?.ExitState();
            _currentState = newState;
            _currentState.EnterState();
            
            GameEvents.OnCameraStateChanged.Invoke(targetType.Name);
        }

        public void SetLockOnTarget(Transform target)
        {
            LockOnTarget = target;
            GameEvents.OnLockOnTargetChanged.Invoke(target);
            
            if (target != null)
            {
                ChangeState<LockOnCameraState>();
            }
            else
            {
                ChangeState<FreeLookCameraState>();
            }
        }
    }
}
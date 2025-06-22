using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Linq;

namespace Camera
{
    [System.Serializable]
    public class CameraStateConfig
    {
        public string stateName;
        public CinemachineCamera virtualCamera;
    }

    public class CameraStateManager : MonoBehaviour
    {
        [Header("States")] [SerializeField] private List<CameraStateConfig> _cameraStates;
        [SerializeField] private string _defaultStateName = "FreeLook";

        [Header("Priorities")] [SerializeField]
        private int _activePriority = 10;

        [SerializeField] private int _inactivePriority = 0;

        private Dictionary<string, CinemachineCamera> _stateMap;
        private CinemachineCamera _activeCamera;

        public CinemachineCamera ActiveCamera => _activeCamera;

        private void Awake()
        {
            _stateMap = _cameraStates
                .Where(s => !string.IsNullOrEmpty(s.stateName) && s.virtualCamera != null)
                .ToDictionary(s => s.stateName, s => s.virtualCamera);

            foreach (var cam in _stateMap.Values)
                cam.Priority = _inactivePriority;

            if (_stateMap.TryGetValue(_defaultStateName, out var defaultCam))
            {
                SwitchState(_defaultStateName);
            }
            else
            {
                UnityEngine.Debug.LogError($"Default state '{_defaultStateName}' not found in camera list.");
            }
        }

        public void SwitchState(string stateName)
        {
            if (!_stateMap.TryGetValue(stateName, out var newCam))
            {
                UnityEngine.Debug.LogError($"Camera state '{stateName}' not found!");
                return;
            }

            if (_activeCamera != null && _activeCamera != newCam)
                _activeCamera.Priority = _inactivePriority;

            newCam.Priority = _activePriority;
            _activeCamera = newCam;
        }
    }
}
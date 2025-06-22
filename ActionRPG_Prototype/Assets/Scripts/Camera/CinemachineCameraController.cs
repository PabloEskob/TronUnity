using UnityEngine;
using Unity.Cinemachine;
using Camera.Interface;
using System.Collections;
using Movement;

namespace Camera
{
    public class CinemachineCameraController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform _followTarget;

        [SerializeField] private Transform _characterTransform;
        [SerializeField] private CameraStateManager _cameraStateManager;
        [SerializeField] private PlayerInputHandler _inputProviderComponent;
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minFOV = 20f;
        [SerializeField] private float maxFOV = 60f;

        private ICameraInputProvider _inputProvider;

        [Header("Lock-On")] [SerializeField] private float _lockOnRadius = 10f;
        [SerializeField] private LayerMask _lockOnLayers;
        private Transform _currentLockTarget;


        private void Awake()
        {
            if (_inputProviderComponent is ICameraInputProvider provider)
                _inputProvider = provider;
            else
                UnityEngine.Debug.LogWarning("Input provider not set or doesn't implement ICameraInputProvider.");
        }

        private void Update()
        {
            if (_inputProvider == null) return;

            if (_inputProvider.IsResetCameraPressed)
                ResetCamera();

            if (_inputProvider.IsLockOnPressed)
                ToggleLockOn();

            if (_currentLockTarget != null)
                MaintainLockOn();

            HandleZoom();
        }

        private void HandleZoom()
        {
            var zoomDelta = _inputProvider.ZoomInput;
            if (Mathf.Abs(zoomDelta) > 0.01f)
            {
                var activeVcam = _cameraStateManager.ActiveCamera;
                if (activeVcam != null)
                {
                    var lens = activeVcam.Lens;
                    lens.FieldOfView = Mathf.Clamp(lens.FieldOfView - zoomDelta * zoomSpeed, minFOV, maxFOV);
                    activeVcam.Lens = lens;
                }
            }
        }

        private void ResetCamera()
        {
            _cameraStateManager.SwitchState("FreeLook");
            StartCoroutine(RecenterAfterTransition());
        }

        private IEnumerator RecenterAfterTransition()
        {
            yield return new WaitForSeconds(0.5f); // wait for blend

            var vcam = _cameraStateManager.ActiveCamera;
            if (vcam == null) yield break;

            var follow = vcam.GetComponent<CinemachineOrbitalFollow>();
            if (follow != null && _characterTransform != null)
            {
                // Обновим heading (горизонтальное направление)
                follow.HorizontalAxis.Value = _characterTransform.eulerAngles.y;

                // Обновим pitch (наклон)
                follow.VerticalAxis.Value = 15f; // например, 15 градусов вверх
            }
        }

        private void ToggleLockOn()
        {
            if (_currentLockTarget != null)
            {
                ClearLockOnTarget();
            }
            else
            {
                var target = FindClosestTarget();
                if (target != null)
                    SetLockOnTarget(target);
            }
        }

        private Transform FindClosestTarget()
        {
            Collider[] hits = Physics.OverlapSphere(_characterTransform.position, _lockOnRadius, _lockOnLayers);
            Transform closest = null;
            float closestSqr = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.transform == _characterTransform) continue;

                float distSqr = (hit.transform.position - _characterTransform.position).sqrMagnitude;
                if (distSqr < closestSqr)
                {
                    closest = hit.transform;
                    closestSqr = distSqr;
                }
            }

            return closest;
        }

        private void SetLockOnTarget(Transform target)
        {
            _currentLockTarget = target;
            _cameraStateManager.SwitchState("LockOn");

            var vcam = _cameraStateManager.ActiveCamera;
            if (vcam != null)
            {
                vcam.LookAt = _currentLockTarget;
            }
        }

        private void ClearLockOnTarget()
        {
            _currentLockTarget = null;
            _cameraStateManager.SwitchState("FreeLook");

            var vcam = _cameraStateManager.ActiveCamera;
            if (vcam != null)
            {
                vcam.LookAt = _followTarget;
            }
        }

        private void MaintainLockOn()
        {
            if (_currentLockTarget == null ||
                !_currentLockTarget.gameObject.activeInHierarchy ||
                Vector3.Distance(_characterTransform.position, _currentLockTarget.position) > _lockOnRadius * 1.5f)
            {
                ClearLockOnTarget();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_characterTransform != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_characterTransform.position, _lockOnRadius);
            }

            if (_currentLockTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_characterTransform.position, _currentLockTarget.position);
            }
        }
    }
}
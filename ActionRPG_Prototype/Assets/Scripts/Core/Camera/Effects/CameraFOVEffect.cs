using System.Collections;
using UnityEngine;

namespace Core.Camera.Effects
{
    public class CameraFOVEffect : MonoBehaviour
    {
        private UnityEngine.Camera _camera;
        private float _originalFOV;
        private Coroutine _currentTransition;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _originalFOV = _camera.fieldOfView;
        }

        public void SetFOV(float targetFOV, float duration)
        {
            if (_currentTransition != null)
                StopCoroutine(_currentTransition);

            _currentTransition = StartCoroutine(TransitionFOV(targetFOV, duration));
        }

        public void ResetFOV(float duration)
        {
            SetFOV(_originalFOV, duration);
        }

        private IEnumerator TransitionFOV(float targetFOV, float duration)
        {
            float startFOV = _camera.fieldOfView;
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _camera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
                yield return null;
            }

            _camera.fieldOfView = targetFOV;
            _currentTransition = null;
        }
    }
}
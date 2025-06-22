using UnityEngine;

namespace Core.Camera.Effects
{
    public class CameraShakeEffect : MonoBehaviour
    {
        [Header("Shake Settings")] [SerializeField]
        private float _traumaDecay = 1f;

        [SerializeField] private float _maxAngle = 10f;
        [SerializeField] private float _maxOffset = 0.5f;

        private float _trauma;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private void Awake()
        {
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;
        }

        private void Update()
        {
            if (_trauma > 0)
            {
                _trauma = Mathf.Max(0, _trauma - _traumaDecay * Time.deltaTime);
                ApplyShake();
            }
            else
            {
                transform.localPosition = _originalPosition;
                transform.localRotation = _originalRotation;
            }
        }

        private void ApplyShake()
        {
            var shake = _trauma * _trauma;

            // Position shake
            var offsetX = _maxOffset * shake * Random.Range(-1f, 1f);
            var offsetY = _maxOffset * shake * Random.Range(-1f, 1f);
            transform.localPosition = _originalPosition + new Vector3(offsetX, offsetY, 0);

            // Rotation shake
            var angleX = _maxAngle * shake * Random.Range(-1f, 1f);
            var angleY = _maxAngle * shake * Random.Range(-1f, 1f);
            var angleZ = _maxAngle * shake * Random.Range(-1f, 1f) * 0.5f;
            transform.localRotation = _originalRotation * Quaternion.Euler(angleX, angleY, angleZ);
        }

        public void TriggerShake(float intensity, float duration = 0)
        {
            _trauma = Mathf.Clamp01(_trauma + intensity);

            if (duration > 0)
            {
                _traumaDecay = 1f / duration;
            }
        }
    }
}
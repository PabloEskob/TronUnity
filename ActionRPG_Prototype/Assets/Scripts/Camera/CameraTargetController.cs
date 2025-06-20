using UnityEngine;

namespace Camera
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] private Transform _character;
        [SerializeField] private float _heightOffset = 1.5f;
        [SerializeField] private float _smoothSpeed = 10f;

        void LateUpdate()
        {
            if (_character is null) return;

            // Плавное следование с офсетом по высоте
            Vector3 targetPosition = _character.position + Vector3.up * _heightOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
        }
    }
}
using UnityEngine;

namespace Core.Scripts.Helpers.Ground
{
    [RequireComponent(typeof(CharacterController))]
    public class GroundDetector : MonoBehaviour, IGroundStateProvider
    {
        [SerializeField] 
        private LayerMask _groundMask = ~0;
        
        [SerializeField, Tooltip("Доп. зазор к skinWidth")]
        private float _castMargin = 0.04f;
        
        [SerializeField] 
        private float _maxSlope = 60f; // °

        private CharacterController _characterController;
        
        public bool IsGrounded { get; private set; }
        public Vector3 GroundNormal { get; private set; } = Vector3.up;

        private void Awake() => _characterController = GetComponent<CharacterController>();

        private void FixedUpdate() => UpdateGrounded();

        private void UpdateGrounded()
        {
            var radius = _characterController.radius * Mathf.Abs(transform.localScale.x);
            var halfH = Mathf.Max(radius, _characterController.height * .5f * Mathf.Abs(transform.localScale.y));
            var center = transform.TransformPoint(_characterController.center);

            var top = center + Vector3.up * (halfH - radius);
            var bottom = center + Vector3.down * (halfH - radius);

            var distance = _characterController.skinWidth + _castMargin;

            if (Physics.CapsuleCast(top, bottom, radius * 0.95f,
                    Vector3.down, out var hit,
                    distance, _groundMask,
                    QueryTriggerInteraction.Ignore))
            {
                IsGrounded = Vector3.Angle(hit.normal, Vector3.up) <= _maxSlope;
                GroundNormal = hit.normal;
            }
            else
            {
                IsGrounded = false;
                GroundNormal = Vector3.up;
            }
        }
    }
}
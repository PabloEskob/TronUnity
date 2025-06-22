using UnityEngine;
using Config.Movement;

namespace Character.Movement
{
    public class MovementPhysics : MonoBehaviour
    {
        [Header("Ground Check")] [SerializeField]
        private Transform _groundCheckPoint;

        [SerializeField] private float _groundCheckRadius = 0.3f;

        private CharacterController _characterController;
        private MovementConfig _config;
        private Vector3 _velocity;
        private float _fallTime;

        public bool IsGrounded { get; private set; }
        public bool IsFalling => !IsGrounded && _velocity.y < 0;
        public float FallTime => _fallTime;
        public Vector3 Velocity => _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _config = GetComponent<MovementController>().Config;
        }

        private void FixedUpdate()
        {
            UpdateGroundCheck();
            ApplyGravity();
            ApplyVelocity();
        }

        private void UpdateGroundCheck()
        {
            var wasGrounded = IsGrounded;

            IsGrounded = Physics.CheckSphere(
                _groundCheckPoint.position,
                _groundCheckRadius,
                _config.groundLayer
            );

            if (IsGrounded && !wasGrounded)
            {
                _fallTime = 0f;
                Land();
            }
            else if (!IsGrounded && wasGrounded)
            {
                StartFalling();
            }

            if (IsFalling)
            {
                _fallTime += Time.fixedDeltaTime;
            }
        }

        private void ApplyGravity()
        {
            if (IsGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Small force to keep grounded
            }
            else
            {
                _velocity.y += _config.gravity * Time.fixedDeltaTime;
            }
        }

        private void ApplyVelocity()
        {
            _characterController.Move(_velocity * Time.fixedDeltaTime);
        }

        public void Jump(float jumpForce)
        {
            if (IsGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * _config.gravity);
            }
        }

        public void AddForce(Vector3 force)
        {
            _velocity += force;
        }

        public void ResetVelocity()
        {
            _velocity = Vector3.zero;
        }

        private void Land()
        {
            // Trigger landing effects based on fall time
            if (_fallTime > 0.5f)
            {
                UnityEngine.Debug.Log("Hard landing!");
            }
        }

        private void StartFalling()
        {
            _fallTime = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (_groundCheckPoint != null)
            {
                Gizmos.color = IsGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
            }
        }
    }
}
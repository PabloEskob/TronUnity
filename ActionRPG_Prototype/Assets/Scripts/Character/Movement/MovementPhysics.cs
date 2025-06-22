using UnityEngine;
using Config.Movement;

namespace Character.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class MovementPhysics : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _gravity      = -25f;
        [SerializeField] private float _terminalVel  = -60f;
        [SerializeField] private LayerMask _groundLayers = ~0;
        [SerializeField] private Vector3 _groundCheckOffset = new(0, 0.1f, 0);
        [SerializeField] private float   _groundCheckRadius = 0.28f;

        private CharacterController _cc;
        public bool   IsGrounded { get; private set; }
        public Vector3 Velocity   { get; private set; }

        private void Awake() => _cc = GetComponent<CharacterController>();
        private void Update() => ApplyGravity();

        private void ApplyGravity()
        {
            GroundCheck();
            if (IsGrounded && Velocity.y < 0) Velocity = new(Velocity.x, -2f, Velocity.z);
            else                             Velocity += Vector3.up * _gravity * Time.deltaTime;
            if (Velocity.y < _terminalVel)   Velocity = new(Velocity.x, _terminalVel, Velocity.z);
            _cc.Move(Velocity * Time.deltaTime);
        }

        public void Jump(float force)
        {
            if (!IsGrounded) return;
            Velocity = new(Velocity.x, force, Velocity.z);
        }

        private void GroundCheck()
        {
            Vector3 pos = transform.position + _groundCheckOffset;
            IsGrounded   = Physics.CheckSphere(pos, _groundCheckRadius, _groundLayers, QueryTriggerInteraction.Ignore);
        }
    }
}
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Scripts.Character.Controller
{
    public abstract class PlayerControllerBase : MonoBehaviour, IInputAxisOwner
    {
        [Tooltip("Ground speed when walking")] public float Speed = 1f;

        [Tooltip("Ground speed when sprinting")]
        public float SprintSpeed = 4;

        [Tooltip("Initial vertical speed when jumping")]
        public float JumpSpeed = 4;

        [Tooltip("Initial vertical speed when sprint-jumping")]
        public float SprintJumpSpeed = 6;

        public Action PreUpdate;
        public Action<Vector3, float> PostUpdate;
        public Action StartJump;
        public Action EndJump;

        [Header("Input Axes")] [Tooltip("X Axis movement.  Value is -1..1.  Controls the sideways movement")]
        public InputAxis MoveX = InputAxis.DefaultMomentary;

        [Tooltip("Z Axis movement.  Value is -1..1. Controls the forward movement")]
        public InputAxis MoveZ = InputAxis.DefaultMomentary;

        [Tooltip("Jump movement.  Value is 0 or 1. Controls the vertical movement")]
        public InputAxis Jump = InputAxis.DefaultMomentary;

        [Tooltip("Sprint movement.  Value is 0 or 1. If 1, then is sprinting")]
        public InputAxis Sprint = InputAxis.DefaultMomentary;

        [Header("Events")] [Tooltip("This event is sent when the player lands after a jump.")]
        public UnityEvent Landed = new();

        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref MoveX, Name = "Move X", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new() { DrivenAxis = () => ref MoveZ, Name = "Move Z", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new() { DrivenAxis = () => ref Jump, Name = "Jump" });
            axes.Add(new() { DrivenAxis = () => ref Sprint, Name = "Sprint" });
        }

        public virtual void SetStrafeMode(bool b)
        {
        }

        public abstract bool IsMoving { get; }
    }

    public class PlayerController : PlayerControllerBase
    {
        [Tooltip("Transition duration (in seconds) when the player changes velocity or rotation.")]
        public float Damping = 0.5f;

        [Tooltip("Makes the player strafe when moving sideways, otherwise it turns to face the direction of motion.")]
        public bool Strafe = false;

        public enum ForwardModes
        {
            Camera,
            Player,
            World
        }

        public enum UpModes
        {
            Player,
            World
        }

        [Tooltip(
            "Reference frame for the input controls:\n<b>Camera</b>: Input forward is camera forward direction.\n<b>Player</b>: Input forward is Player's forward direction.\n<b>World</b>: Input forward is World forward direction.")]
        public ForwardModes InputForward = ForwardModes.Camera;

        [Tooltip("Up direction for computing motion:\n<b>Player</b>: Move in the Player's local XZ plane.\n<b>World</b>: Move in global XZ plane.")]
        public UpModes UpMode = UpModes.World;

        [Tooltip("If non-null, take the input frame from this camera instead of Camera.main. Useful for split-screen games.")]
        public Camera CameraOverride;

        [Tooltip("Layers to include in ground detection via Casts.")]
        public LayerMask GroundLayers = 1;

        [Tooltip("Force of gravity in the down direction (m/s^2)")]
        public float Gravity = 10;

        [Header("Ground Detection")] [Tooltip("Additional distance added to CharacterController.skinWidth when CapsuleCasting for ground.")]
        public float GroundCastMargin = 0.04f;

        [Tooltip("Maximum slope angle (degrees) that is still considered ground.")]
        public float MaxGroundSlope = 60f;

        private const float kDelayBeforeInferringJump = 0.3f;
        private float _timeLastGrounded = 0;

        private Vector3 _currentVelocityXZ;
        private Vector3 _lastInput;
        private float _currentVelocityY;
        private bool _isSprinting;
        private bool _isJumping;
        private CharacterController _controller; // optional

        // Variables to prevent gimbal lock when character is inverted relative to input frame
        private bool _inTopHemisphere = true;
        private float _timeInHemisphere = 100;
        private Vector3 _lastRawInput;
        private readonly Quaternion _upSideDown = Quaternion.AngleAxis(180, Vector3.left);
        private Vector3 UpDirection => UpMode == UpModes.World ? Vector3.up : transform.up;

        public override void SetStrafeMode(bool b) => Strafe = b;
        public override bool IsMoving => _lastInput.sqrMagnitude > 0.01f;

        public bool IsSprinting => _isSprinting;
        public bool IsJumping => _isJumping;
        public Camera Camera => CameraOverride == null ? Camera.main : CameraOverride;

        public bool IsGrounded() => CapsuleGroundCheck();

        void Start() => TryGetComponent(out _controller);

        private void OnEnable()
        {
            _currentVelocityY = 0;
            _isSprinting = false;
            _isJumping = false;
            _timeLastGrounded = Time.time;
        }

        private void Update()
        {
            PreUpdate?.Invoke();

            // Process Jump and gravity
            bool justLanded = ProcessJump();

            // Get the reference frame for the input
            var rawInput = new Vector3(MoveX.Value, 0, MoveZ.Value);
            var inputFrame = GetInputFrame(Vector3.Dot(rawInput, _lastRawInput) < 0.8f);
            _lastRawInput = rawInput;

            // Read the input from the user and put it in the input frame
            _lastInput = inputFrame * rawInput;
            if (_lastInput.sqrMagnitude > 1)
                _lastInput.Normalize();

            // Compute the new velocity and move the player, but only if not mid-jump
            if (!_isJumping)
            {
                _isSprinting = Sprint.Value > 0.5f;
                var desiredVelocity = _lastInput * (_isSprinting ? SprintSpeed : Speed);
                var damping = justLanded ? 0 : Damping;
                if (Vector3.Angle(_currentVelocityXZ, desiredVelocity) < 100)
                    _currentVelocityXZ = Vector3.Slerp(
                        _currentVelocityXZ, desiredVelocity,
                        Damper.Damp(1, damping, Time.deltaTime));
                else
                    _currentVelocityXZ += Damper.Damp(
                        desiredVelocity - _currentVelocityXZ, damping, Time.deltaTime);
            }

            // Apply the position change
            ApplyMotion();

            // If not strafing, rotate the player to face movement direction
            if (!Strafe && _currentVelocityXZ.sqrMagnitude > 0.001f)
            {
                var fwd = inputFrame * Vector3.forward;
                var qA = transform.rotation;
                var qB = Quaternion.LookRotation(
                    (InputForward == ForwardModes.Player && Vector3.Dot(fwd, _currentVelocityXZ) < 0)
                        ? -_currentVelocityXZ
                        : _currentVelocityXZ, UpDirection);
                var damping = justLanded ? 0 : Damping;
                transform.rotation = Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.deltaTime));
            }

            if (PostUpdate != null)
            {
                // Get local-space velocity
                var vel = Quaternion.Inverse(transform.rotation) * _currentVelocityXZ;
                vel.y = _currentVelocityY;
                PostUpdate(vel, _isSprinting ? JumpSpeed / SprintJumpSpeed : 1);
            }
        }

        private bool CapsuleGroundCheck()
        {
            var up = UpDirection;

            // If the player uses a CharacterController, derive the cast from its parameters
            if (_controller != null)
            {
                // Radius & height scaled by transform scale
                float radius = _controller.radius * Mathf.Abs(transform.localScale.x);
                float halfH = Mathf.Max(radius, _controller.height * 0.5f * Mathf.Abs(transform.localScale.y));
                Vector3 centerWorld = transform.TransformPoint(_controller.center);

                Vector3 top = centerWorld + up * (halfH - radius);
                Vector3 bottom = centerWorld - up * (halfH - radius);

                float castDistance = _controller.skinWidth + GroundCastMargin;

                if (Physics.CapsuleCast(top, bottom, radius * 0.95f, -up, out var hit,
                        castDistance, GroundLayers, QueryTriggerInteraction.Ignore))
                {
                    return Vector3.Angle(hit.normal, up) <= MaxGroundSlope;
                }

                return false;
            }

            // No CharacterController – fallback to small raycast distance
            return GetDistanceFromGround(transform.position, up, 0.2f) < 0.01f;
        }


        private Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            var frame = Quaternion.identity;
            switch (InputForward)
            {
                case ForwardModes.Camera: frame = Camera.transform.rotation; break;
                case ForwardModes.Player: return transform.rotation;
                case ForwardModes.World: break;
            }

            var playerUp = transform.up;
            var up = frame * Vector3.up;

            const float blendTime = 2f;
            _timeInHemisphere += Time.deltaTime;
            bool inTopHemisphere = Vector3.Dot(up, playerUp) >= 0;
            if (inTopHemisphere != _inTopHemisphere)
            {
                _inTopHemisphere = inTopHemisphere;
                _timeInHemisphere = Mathf.Max(0, blendTime - _timeInHemisphere);
            }

            var axis = Vector3.Cross(up, playerUp);
            if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
                return frame;

            var angle = UnityVectorExtensions.SignedAngle(up, playerUp, axis);
            var frameA = Quaternion.AngleAxis(angle, axis) * frame;

            Quaternion frameB = frameA;
            if (!inTopHemisphere || _timeInHemisphere < blendTime)
            {
                frameB = frame * _upSideDown;
                var axisB = Vector3.Cross(frameB * Vector3.up, playerUp);
                if (axisB.sqrMagnitude > 0.001f)
                    frameB = Quaternion.AngleAxis(180f - angle, axisB) * frameB;
            }

            if (inputDirectionChanged)
                _timeInHemisphere = blendTime;

            if (_timeInHemisphere >= blendTime)
                return inTopHemisphere ? frameA : frameB;

            if (inTopHemisphere)
                return Quaternion.Slerp(frameB, frameA, _timeInHemisphere / blendTime);
            return Quaternion.Slerp(frameA, frameB, _timeInHemisphere / blendTime);
        }

        bool ProcessJump()
        {
            bool justLanded = false;
            var now = Time.time;
            bool grounded = IsGrounded();

            _currentVelocityY -= Gravity * Time.deltaTime;

            if (!_isJumping)
            {
                // Process jump command
                if (grounded && Jump.Value > 0.01f)
                {
                    _isJumping = true;
                    _currentVelocityY = _isSprinting ? SprintJumpSpeed : JumpSpeed;
                }

                // If we are falling, assume the jump pose
                if (!grounded && now - _timeLastGrounded > kDelayBeforeInferringJump)
                    _isJumping = true;

                if (_isJumping)
                {
                    StartJump?.Invoke();
                    grounded = false;
                }
            }

            if (grounded)
            {
                _timeLastGrounded = Time.time;
                _currentVelocityY = 0;

                // If we were jumping, complete the jump
                if (_isJumping)
                {
                    EndJump?.Invoke();
                    _isJumping = false;
                    justLanded = true;
                    Landed.Invoke();
                }
            }

            return justLanded;
        }

        void ApplyMotion()
        {
            if (_controller)
                _controller.Move((_currentVelocityY * UpDirection + _currentVelocityXZ) * Time.deltaTime);
            else
            {
                var pos = transform.position + _currentVelocityXZ * Time.deltaTime;

                // Don't fall below ground
                var up = UpDirection;
                var altitude = GetDistanceFromGround(pos, up, 10);
                if (altitude < 0 && _currentVelocityY <= 0)
                {
                    pos -= altitude * up;
                    _currentVelocityY = 0;
                }
                else if (_currentVelocityY < 0)
                {
                    var dy = -_currentVelocityY * Time.deltaTime;
                    if (dy > altitude)
                    {
                        pos -= altitude * up;
                        _currentVelocityY = 0;
                    }
                }

                transform.position = pos + up * (_currentVelocityY * Time.deltaTime);
            }
        }

        float GetDistanceFromGround(Vector3 pos, Vector3 up, float max)
        {
            float kExtraHeight = _controller == null ? 2 : 0; // start a little above the player in case it's moving down fast
            if (Physics.Raycast(pos + up * kExtraHeight, -up, out var hit,
                    max + kExtraHeight, GroundLayers, QueryTriggerInteraction.Ignore))
                return hit.distance - kExtraHeight;
            return max + 1;
        }
    }
}
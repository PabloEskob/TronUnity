using System;
using Core.Scripts.Infrastructure;
using Core.Scripts.Services.Input;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.Character
{
    public class HeroMove : MonoBehaviour
    {
        [Header("Movement")]
        public CharacterController CharacterController;
        public float MovementSpeed = 1f;
        public float SprintSpeed = 4f;
        public bool IsSprinting;

        [Tooltip("Плавность изменения скорости и поворота")]
        public float Damping = 0.5f;

        [Header("Rotation")]
        [Tooltip("Если false, персонаж поворачивается в направлении движения")]
        public bool Strafe = false;
        
        public bool IsMoving => _currentVelocityXZ.sqrMagnitude > 0.01f;
        
        public Action PreUpdate;
        public Action<Vector3, float> PostUpdate;

        private IInputService _inputService;
        private Camera _camera;
        private Vector3 _lastInput;
        private Vector3 _currentVelocityXZ;
        private float _currentVelocityY;
        
        private void Awake()
        {
            _inputService = Game.InputService;
            if (!CharacterController)
                CharacterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            PreUpdate?.Invoke();
            
            Vector3 rawInput = new Vector3(_inputService.Axis.x, 0, _inputService.Axis.y);

            Quaternion inputFrame = GetCameraInputFrame();

            _lastInput = inputFrame * rawInput;

            if (_lastInput.sqrMagnitude > 1)
                _lastInput.Normalize();

            float targetSpeed = IsSprinting ? SprintSpeed : MovementSpeed;
            Vector3 desiredVelocity = _lastInput * targetSpeed;

            if (Damping > 0)
            {
                if (Vector3.Angle(_currentVelocityXZ, desiredVelocity) < 100)
                {
                    _currentVelocityXZ = Vector3.Slerp(
                        _currentVelocityXZ,
                        desiredVelocity,
                        Damper.Damp(1, Damping, Time.deltaTime));
                }
                else
                {
                    _currentVelocityXZ += Damper.Damp(
                        desiredVelocity - _currentVelocityXZ,
                        Damping,
                        Time.deltaTime);
                }
            }
            else
            {
                _currentVelocityXZ = desiredVelocity;
            }

            Vector3 motion = (_currentVelocityXZ + _currentVelocityY * Vector3.up) * Time.deltaTime;
            motion += Physics.gravity * Time.deltaTime;

            CharacterController.Move(motion);

            if (!Strafe && _currentVelocityXZ.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_currentVelocityXZ, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Damper.Damp(1, Damping, Time.deltaTime));
            }
            
            if (PostUpdate != null)
            {
                Vector3 localVelocity = Quaternion.Inverse(transform.rotation) * _currentVelocityXZ;
                localVelocity.y = _currentVelocityY;
                PostUpdate(localVelocity, 1f);
            }
        }
        
        public void SetStrafeMode(bool strafe)
        {
            Strafe = strafe;
        }

        private Quaternion GetCameraInputFrame()
        {
            Vector3 cameraForward = _camera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            if (cameraForward.sqrMagnitude < 0.001f)
            {
                cameraForward = _camera.transform.right;
            }

            return Quaternion.LookRotation(cameraForward, Vector3.up);
        }
    }
}
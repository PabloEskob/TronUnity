using Core.Scripts.Data;
using Core.Scripts.Services.Input;
using Core.Scripts.Services.PersistentProgress;
using ECM2;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.Scripts.Character.Hero
{
    public class HeroMove : MonoBehaviour, ISavedProgress
    {
        [Header("Cinemachine")] [Tooltip("The CM virtual Camera following the target.")]
        public CinemachineCamera FollowCamera;

        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow.")]
        public GameObject FollowTarget;

        [Tooltip("The default distance behind the Follow target.")] [SerializeField]
        public float FollowDistance = 5.0f;

        [Tooltip("The minimum distance to Follow target.")] [SerializeField]
        public float FollowMinDistance = 2.0f;

        [Tooltip("The maximum distance to Follow target.")] [SerializeField]
        public float FollowMaxDistance = 10.0f;

        [Tooltip("How far in degrees can you move the camera up.")]
        public float MaxPitch = 80.0f;

        [Tooltip("How far in degrees can you move the camera down.")]
        public float MinPitch = -80.0f;

        [Space(15.0f)] public bool InvertLook = true;

        [Tooltip("Mouse look sensitivity")] public Vector2 LookSensitivity = new(1.5f, 1.25f);

        private ECM2.Character _character;
        private float _cameraTargetYaw;
        private float _cameraTargetPitch;
        private CinemachineThirdPersonFollow _cmThirdPersonFollow;
        private IInputService _inputService;
        private float _followDistanceSmoothVelocity;

        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            _character = GetComponent<ECM2.Character>();

            FollowCamera = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<CinemachineCamera>();

            FollowCamera.Follow = FollowTarget.transform;

            _cmThirdPersonFollow = FollowCamera.GetComponent<CinemachineThirdPersonFollow>();

            if (_cmThirdPersonFollow)
            {
                _cmThirdPersonFollow.CameraDistance = FollowDistance;
            }
        }

        private void Start()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;

            _character.camera = Camera.main;
            
        }

        private void Update()
        {
            HandleMovementInput();
            HandleLookInput();
            HandleZoomInput();
        }

        private void LateUpdate()
        {
            UpdateCamera();
        }

        private void HandleMovementInput()
        {
            Vector3 dir = new Vector3(_inputService.AxisMove.x, 0, _inputService.AxisMove.y);

            if (_character.camera)
                dir = dir.relativeTo(_character.cameraTransform);

            _character.SetMovementDirection(dir);
        }

        private void HandleLookInput()
        {
            AddControlYawInput(_inputService.AxisLook.x * LookSensitivity.x);
            AddControlPitchInput(_inputService.AxisLook.y * LookSensitivity.y, MinPitch, MaxPitch);
        }

        private void HandleZoomInput()
        {
            AddControlZoomInput(Input.GetAxisRaw("Mouse ScrollWheel"));
        }


        public void UpdateProgress(PlayerProgress playerProgress) =>
            playerProgress.WorldData.PositionOnLevel = new PositionOnLevel(CurrentLevel(), transform.position.AsVectorData());

        public void LoadProgress(PlayerProgress playerProgress)
        {
            if (CurrentLevel() == playerProgress.WorldData.PositionOnLevel.Level)
            {
                var savedPosition = playerProgress.WorldData.PositionOnLevel.Position;
                if (savedPosition != null)
                    Warp(to: savedPosition);
            }
        }

        private void AddControlYawInput(float value, float minValue = -180.0f, float maxValue = 180.0f)
        {
            if (value != 0.0f) _cameraTargetYaw = MathLib.ClampAngle(_cameraTargetYaw + value, minValue, maxValue);
        }

        private void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f)
        {
            if (value == 0.0f)
                return;

            if (InvertLook)
                value = -value;

            _cameraTargetPitch = MathLib.ClampAngle(_cameraTargetPitch + value, minValue, maxValue);
        }

        protected virtual void AddControlZoomInput(float value)
        {
            FollowDistance = Mathf.Clamp(FollowDistance - value, FollowMinDistance, FollowMaxDistance);
        }

        private void Warp(Vector3Data to)
        {
            _character.SetPosition(to.AsUnityVector().AddY(_character.height), updateGround: true);
        }

        private static string CurrentLevel() =>
            SceneManager.GetActiveScene().name;

        private void UpdateCamera()
        {
            FollowTarget.transform.rotation = Quaternion.Euler(_cameraTargetPitch, _cameraTargetYaw, 0.0f);

            _cmThirdPersonFollow.CameraDistance =
                Mathf.SmoothDamp(_cmThirdPersonFollow.CameraDistance, FollowDistance, ref _followDistanceSmoothVelocity, 0.1f);
        }
    }
}
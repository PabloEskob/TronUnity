using Config.Camera;
using Core.Input.Interfaces;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Core.Camera
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class GenshinCameraController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        CameraConfig config;

        [SerializeField] Transform player;

        // Components
        CinemachineCamera vcam;
        CinemachineThirdPersonFollow thirdPerson;

        // Input
        ICameraInput input;

        // Rotation state
        float yaw;
        float pitch;
        float yawVelocity;
        float pitchVelocity;

        // Zoom state
        float currentDistance;
        float targetDistance;
        float zoomVelocity;

        // Auto rotation
        float timeSinceLastInput;

        [Inject]
        public void Construct(ICameraInput cameraInput)
        {
            input = cameraInput;
        }

        void Awake()
        {
            vcam = GetComponent<CinemachineCamera>();
            thirdPerson = vcam.GetComponent<CinemachineThirdPersonFollow>();

            // Применяем начальные настройки
            ApplyConfig();

            // Инициализируем состояние
            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
            currentDistance = config.defaultDistance;
            targetDistance = currentDistance;
        }

        void ApplyConfig()
        {
            if (thirdPerson != null)
            {
                thirdPerson.CameraDistance = config.defaultDistance;
                thirdPerson.ShoulderOffset = config.shoulderOffset;
                thirdPerson.CameraSide = config.cameraSide;
                thirdPerson.VerticalArmLength = config.verticalArmLength;
            }
        }

        void LateUpdate()
        {
            HandleRotation();
            HandleZoom();
            HandleAutoRotation();
        }

        void HandleRotation()
        {
            Vector2 lookInput = input.LookInput;

            if (lookInput.magnitude > 0.01f)
            {
                // Применяем чувствительность
                float sensitivity = config.mouseSensitivity;

                yaw += lookInput.x * sensitivity;
                pitch -= lookInput.y * sensitivity * (config.invertY ? -1 : 1);

                // Ограничиваем pitch
                pitch = Mathf.Clamp(pitch, config.pitchMin, config.pitchMax);

                timeSinceLastInput = 0f;
            }
            else
            {
                timeSinceLastInput += Time.deltaTime;
            }

            // Сброс камеры
            if (input.IsResetCameraPressed)
            {
                yaw = player.eulerAngles.y;
                pitch = 0f;
            }

            // Применяем вращение
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }

        void HandleZoom()
        {
            // Обновляем целевую дистанцию
            if (Mathf.Abs(input.ZoomInput) > 0.01f)
            {
                targetDistance = Mathf.Clamp(
                    targetDistance - input.ZoomInput * config.zoomSpeed,
                    config.minDistance,
                    config.maxDistance
                );
            }

            // Плавно интерполируем
            currentDistance = Mathf.SmoothDamp(
                currentDistance,
                targetDistance,
                ref zoomVelocity,
                config.zoomSmoothTime
            );

            if (thirdPerson != null)
            {
                thirdPerson.CameraDistance = currentDistance;
            }
        }

        void HandleAutoRotation()
        {
            if (!config.enableAutoRotation) return;

            // Автоповорот за спину при движении
            if (timeSinceLastInput > config.autoRotationDelay && IsPlayerMoving())
            {
                float targetYaw = player.eulerAngles.y;
                yaw = Mathf.LerpAngle(yaw, targetYaw, config.autoRotationSpeed * Time.deltaTime);
                pitch = Mathf.Lerp(pitch, 0f, config.autoRotationSpeed * Time.deltaTime);
            }
        }

        bool IsPlayerMoving()
        {
            // Простая проверка движения
            return player.GetComponent<Rigidbody>()?.linearVelocity.magnitude > 0.1f;
        }

        public void SetPlayer(Transform newPlayer)
        {
            player = newPlayer;

            // Сразу устанавливаем камеру за спину игрока
            if (player != null)
            {
                yaw = player.eulerAngles.y;
                pitch = 0f;
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Character.Movement
{
    /// <summary>Обёртка над CharacterController с надёжным isGrounded.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class MovementPhysics : MonoBehaviour
    {
        [Header("Ground Check (SphereCast)")] [SerializeField]
        LayerMask groundMaskSphereCast = ~0; // Переименовал для ясности

        [Tooltip("Допуск, когда всё ещё считаем, что стоим (сек)")] [Range(0, 0.5f)]
        public float coyoteTime = 0.1f;

        [Tooltip("Смещение SphereCast вверх от низа капсулы")]
        public float sphereCastRayOffset = 0.05f; // Переименовал для ясности

        [Header("Ground Check (Raycast - из SimplePlayerController)")] [Tooltip("Слои для Raycast проверки земли")]
        public LayerMask groundMaskRaycast = 1; // Как в SimplePlayerController

        [Tooltip("Максимальная дистанция для Raycast проверки земли")]
        public float raycastMaxDistance = 10f;

        public enum UpModes
        {
            Player,
            World
        };

        [Tooltip("Направление 'вверх' для Raycast проверки:\n"
                 + "Player: Локальная ось Y игрока.\n"
                 + "World: Глобальная ось Y.")]
        public UpModes RaycastUpMode = UpModes.World;


        [Header("Gravity")] public float gravity = -9.81f;

        /* ─ Runtime ─ */
        public bool IsGrounded { get; private set; } // Это будет обновляться через SphereCast
        public Vector3 Velocity { get; private set; }

        CharacterController cc;
        float lastGroundedTime;

        // Твой метод, который ты хотел, теперь будет использовать Raycast-логику
        public bool IsGroundedByRaycast() => GetDistanceFromGround_Raycast(transform.position, CurrentRaycastUpDirection, raycastMaxDistance) < 0.01f;

        // Вспомогательное свойство для определения направления "вверх" для Raycast
        Vector3 CurrentRaycastUpDirection => RaycastUpMode == UpModes.World ? Vector3.up : transform.up;

        void Awake() => cc = GetComponent<CharacterController>();

        void FixedUpdate()
        {
            /* 1. Обновляем вертикальную скорость */
            Velocity += Vector3.up * gravity * Time.deltaTime; // Гравитация всегда по мировому Y

            /* 2. Двигаемся CC */
            cc.Move(Velocity * Time.deltaTime);

            /* 3. Кастуем SphereCast от центра вниз (основная проверка на землю) */
            // Используем мировое Vector3.up/down для SphereCast, как было изначально
            Vector3 sphereOrigin = transform.position + Vector3.up * (sphereCastRayOffset + 0.01f);
            float sphereRayLen = sphereCastRayOffset + 0.1f; // ~5 см
            bool hitSphere = Physics.SphereCast(sphereOrigin, cc.radius * 0.95f,
                Vector3.down, out var hitInfo,
                sphereRayLen, groundMaskSphereCast,
                QueryTriggerInteraction.Ignore);

            if (hitSphere && Vector3.Angle(hitInfo.normal, Vector3.up) < 60f) // Проверка угла по мировому Vector3.up
            {
                lastGroundedTime = Time.time;
                // прилипание к поверхности
                float penetration = sphereCastRayOffset - hitInfo.distance;
                if (penetration > 0f)
                    cc.Move(Vector3.down * penetration); // Прилипание по мировому Vector3.down
                Velocity = new Vector3(Velocity.x, 0, Velocity.z);
            }

            IsGrounded = Time.time - lastGroundedTime <= coyoteTime; // IsGrounded управляется SphereCast'ом
        }

        public void Jump(float impulse)
        {
            // одиночный импульс вверх
            Velocity = new Vector3(Velocity.x, impulse, Velocity.z);
            IsGrounded = false; // Сразу считаем, что не на земле
            lastGroundedTime = Time.time - coyoteTime - 0.01f; // Сбрасываем таймер, чтобы coyote time не сработал сразу
        }

        public float GetDistanceFromGround_Raycast(Vector3 pos, Vector3 up, float maxDistance)
        {
            float kExtraHeight = cc.skinWidth > 0.001f ? cc.skinWidth : 0.01f; // Небольшой отступ, чтобы луч не начался внутри коллайдера


            Vector3 rayStartPoint = pos + up * kExtraHeight;

            if (Physics.Raycast(rayStartPoint, -up, out var hit,
                    maxDistance + kExtraHeight, groundMaskRaycast, QueryTriggerInteraction.Ignore))
            {
                return hit.distance - kExtraHeight;
            }

            return maxDistance + 1; // Земля не найдена в пределах maxDistance
        }
    }
}
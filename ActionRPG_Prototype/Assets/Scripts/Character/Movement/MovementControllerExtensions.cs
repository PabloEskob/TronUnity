using UnityEngine;

namespace Character.Movement
{
    public static class MovementControllerExtensions
    {
        /// <summary>
        /// Плавное движение с интерполяцией
        /// </summary>
        public static void MoveSmooth(this MovementController controller,
            Vector3 direction, float speed, float smoothTime)
        {
            var smoothedDirection = Vector3.Lerp(
                controller.LastMovementDirection,
                direction,
                smoothTime * Time.deltaTime
            );

            controller.Move(smoothedDirection, speed);
        }

        /// <summary>
        /// Проверка возможности выполнить действие основываясь на текущей скорости
        /// </summary>
        public static bool CanPerformAction(this MovementController controller,
            float requiredSpeed = 0.5f)
        {
            return controller.CurrentSpeed <= requiredSpeed;
        }

        /// <summary>
        /// Остановка с замедлением
        /// </summary>
        public static void StopWithDeceleration(this MovementController controller,
            float decelerationRate = 5f)
        {
            var currentVelocity = controller.CurrentVelocity;
            currentVelocity.y = 0; // Сохраняем вертикальную скорость

            var deceleratedVelocity = Vector3.Lerp(
                currentVelocity,
                Vector3.zero,
                decelerationRate * Time.deltaTime
            );

            if (deceleratedVelocity.magnitude < 0.1f)
            {
                controller.Stop();
            }
            else
            {
                controller.Move(deceleratedVelocity.normalized, deceleratedVelocity.magnitude);
            }
        }
    }
}
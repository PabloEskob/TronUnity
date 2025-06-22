using UnityEngine;

namespace Camera.Interface
{
    public interface ICameraInputProvider
    {
        Vector2 LookInput { get; }
        float ZoomInput { get; } // Может быть значение скролла или оси
        bool IsResetCameraPressed { get; }
        bool IsLockOnPressed { get; } // Пример
    }
}
using UnityEngine;

namespace Core.Input.Interfaces
{
    public interface ICameraInput
    {
        Vector2 LookInput { get; }
        float ZoomInput   { get; }
        bool  IsResetCameraPressed { get; }
        bool  IsLockOnPressed      { get; }
    }
}
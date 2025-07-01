using UnityEngine;

namespace Core.Scripts.Services.Input
{
    public interface IInputService
    {
        Vector2 AxisMove { get; }
        
        Vector2 AxisLook { get; }

        bool IsAttackButtonUp();
    }
}
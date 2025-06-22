using UnityEngine;

namespace Core.Input.Interfaces
{
    public interface IMovementInput
    {
        Vector2 MovementVector { get; }
        bool IsRunning { get; }
        bool IsDodging { get; }
        bool IsJumping { get; }
        bool IsAttacking { get; }
    }
}
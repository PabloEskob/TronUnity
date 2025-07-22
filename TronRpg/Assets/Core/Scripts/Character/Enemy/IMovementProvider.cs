using UnityEngine;
using System;

namespace Core.Scripts.Character.Enemy
{
    /// <summary>
    /// Интерфейс для предоставления данных о движении, абстрагирует FollowerEntity.
    /// Следует DIP, избегая прямой зависимости от A* Pathfinding.
    /// </summary>
    public interface IMovementProvider
    {
        Vector3 Velocity { get; }
        float MaxSpeed { get; }
        bool ReachedEndOfPath { get; }

        event Action OnPathCompleted;
        event Action<Vector3> OnVelocityChanged;
        
        void StopMovement();
        void ResumeMovement(float originalMaxSpeed);
    }
}
using UnityEngine;

namespace Movement.Interface
{
    public interface IVelocityProvider
    {
        Vector3 Velocity { get; }
        bool IsAvailable { get; }
    }
}
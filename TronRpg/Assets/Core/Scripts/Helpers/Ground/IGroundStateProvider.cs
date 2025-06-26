using UnityEngine;

namespace Core.Scripts.Helpers.Ground
{
    public interface IGroundStateProvider
    {
        bool IsGrounded { get; } 
        Vector3 GroundNormal { get; } 
    }
}
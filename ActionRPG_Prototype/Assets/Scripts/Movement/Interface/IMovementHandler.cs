using UnityEngine;

namespace Assets.Scripts.Movement.Interface
{
    public interface IMovementHandler
    {
        void Move(Vector3 direction, float speed);
        void Rotate(Vector3 direction, float rotationSpeed);
        void Stop();
    }
}
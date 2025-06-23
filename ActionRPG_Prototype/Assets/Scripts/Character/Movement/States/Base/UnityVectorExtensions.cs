// UnityVectorExtensions.cs

using UnityEngine;

namespace Character.Movement.States.Base
{
    public static class UnityVectorExtensions
    {
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float angle = Vector3.Angle(from, to);
            float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
            return angle * sign;
        }
        
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            float distance = -Vector3.Dot(planeNormal.normalized, vector);
            return vector + planeNormal * distance;
        }
    }
}
﻿using UnityEngine;

namespace Core.Scripts.Data
{
    public static class DataExtensions
    {
        public static Vector3Data AsVectorData(this Vector3 vector) =>
            new Vector3Data(vector.x, vector.y, vector.z);

        public static Vector3 AsUnityVector(this Vector3Data vector3Data) =>
            new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);

        public static Vector3 AddY(this Vector3 vector, float y) => 
            new(vector.x, vector.y += y, vector.z);

        public static T ToDeserialized<T>(this string json) =>
            JsonUtility.FromJson<T>(json);

        public static string ToJson(this object obj) =>
            JsonUtility.ToJson(obj);
    }
}
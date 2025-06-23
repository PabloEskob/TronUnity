using UnityEngine;

namespace DI
{
    [CreateAssetMenu(fileName = nameof(GameConfiguration), menuName = "Config/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Player")]
        public GameObject PlayerPrefab;
        [Min(0f)] public float PlayerSpawnHeight = 1f;

        [Header("Camera")]
        [Min(0f)] public float CameraDistance = 10f;
        [Min(0f)] public float CameraHeight   = 2f;
    }
}
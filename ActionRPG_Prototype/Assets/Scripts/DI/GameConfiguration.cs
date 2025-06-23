using System;
using UnityEngine;

namespace DI
{
    [CreateAssetMenu(menuName = "Config/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        [Serializable]
        public struct PlayerCfg
        {
            public GameObject Prefab;
            [Min(0)] public float SpawnHeight;
        }

        [Serializable]
        public struct CameraCfg
        {
            [Min(0)] public float Distance;
            [Min(0)] public float Height;
        }

        [Header("Player")] public PlayerCfg Player;
        [Header("Camera")] public CameraCfg Camera;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Player.Prefab == null)
                Debug.LogError("[GameConfiguration] Player Prefab is null", this);
        }
#endif
    }
}
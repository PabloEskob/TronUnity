using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(menuName = "Pavel/Footsteps/Surface Profile", fileName = "FootstepProfile_Grass")]
    public sealed class FootstepProfile : ScriptableObject
    {
        public PhysicsMaterial physicMaterial; // основной идентификатор

        [Tooltip("Fallback Tag, если PhysicMaterial = null")]
        public string surfaceTag = "Untagged";

        [Tooltip("Audio variations")] public AudioClip[] clips;
        public bool createDust = true;
        public GameObject dustPrefab; // pooled
        public GameObject footprintPrefab; // pooled / optional
    }
}
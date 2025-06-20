// CharacterConfig.cs

using Config.Movement;
using UnityEngine;

namespace Config.Character
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Character/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        [Header("Movement")] public MovementConfig movementConfig;

        [Header("Combat")] public float health = 100f;
        public float attackDamage = 10f;
        public float defense = 5f;

        [Header("Visual")] public RuntimeAnimatorController animatorController;
        public Avatar characterAvatar;

        [Header("Audio")] public AudioClip[] voiceClips;
        public AudioClip[] hurtSounds;
    }
}
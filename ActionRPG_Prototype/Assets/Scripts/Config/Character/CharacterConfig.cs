using UnityEngine;

namespace Character.Core
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Character/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        [Header("Identity")] public string characterName = "Character";
        public CharacterType characterType = CharacterType.Player;

        [Header("Stats")] public float maxHealth = 100f;
        public float maxStamina = 100f;
        public float staminaRegenRate = 10f;
        public float defense = 10f;

        [Header("Combat")] public float baseDamage = 10f;
        public float criticalChance = 0.1f;
        public float criticalMultiplier = 2f;

        [Header("Movement")] public Config.Movement.MovementConfig movementConfig;

        [Header("Animations")] public RuntimeAnimatorController animatorController;
        public Avatar avatar;
    }

    public enum CharacterType
    {
        Player,
        Enemy,
        NPC,
        Boss
    }
}
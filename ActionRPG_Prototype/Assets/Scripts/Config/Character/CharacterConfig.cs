using Config.Movement;
using UnityEngine;

namespace Character.Core
{
    [CreateAssetMenu(menuName="Pavel/Character Config", fileName="CharacterConfig")]
    public sealed class CharacterConfig : ScriptableObject
    {
        public float baseHealth   = 100;
        public float baseStamina  = 100;
        public MovementConfig movement;
    }
}
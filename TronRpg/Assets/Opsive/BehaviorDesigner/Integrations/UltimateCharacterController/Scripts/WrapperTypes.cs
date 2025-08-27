/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using UnityEngine;

    /// <summary>
    /// A wrapper for the CategoryID.
    /// </summary>
    public struct CategoryID
    {
        [Tooltip("The ID of the category.")]
        public uint ID;
    }

    /// <summary>
    /// A wrapper for the Ability string.
    /// </summary>
    public struct AbilityString
    {
        [Tooltip("The type of the ability.")]
        public string Type;
    }

    /// <summary>
    /// A wrapper for the ItemSetAbility string.
    /// </summary>
    public struct ItemSetAbilityString
    {
        [Tooltip("The type of the ability.")]
        public string Type;
    }

    /// <summary>
    /// A wrapper for the Effect string.
    /// </summary>
    public struct EffectString
    {
        [Tooltip("The type of the effect.")]
        public string Type;
    }
}
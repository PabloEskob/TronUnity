/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.UltimateCharacterController.Character;
    using Opsive.UltimateCharacterController.Character.Abilities.Items;
    using Opsive.Shared.Game;
    using Opsive.Shared.Utility;
    using UnityEngine;

    [NodeDescription("Tries to start the ItemSet ability.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class StartItemSetAbility : TargetGameObjectAction
    {
        [Tooltip("The name of the ItemSet ability.")]
        public SharedVariable<ItemSetAbilityString> m_AbilityType;
        [Tooltip("The category that the ability should respond to.")]
        public SharedVariable<CategoryID> m_CategoryID;

        private GameObject m_PrevTarget;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private ItemSetAbilityBase m_ItemSetAbility;

        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_CharacterLocomotion = gameObject.GetCachedComponent<UltimateCharacterLocomotion>();
                // Find the specified ability.
                var abilities = m_CharacterLocomotion.GetAbilities(TypeUtility.GetType(m_AbilityType.Value.Type)) as ItemSetAbilityBase[];
                // The category ID must match.
                for (int i = 0; i < abilities.Length; ++i) {
                    if (abilities[i].ItemSetCategoryID == m_CategoryID.Value.ID) {
                        m_ItemSetAbility = abilities[i];
                        break;
                    }
                }
                if (m_ItemSetAbility == null) {
                    Debug.LogWarning("Error: Unable to find an ItemSet ability with category ID " + m_CategoryID.Value + ".");
                    return;
                }
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Tries to start the ItemSet ability.
        /// </summary>
        /// <returns>Success if the ability was started.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_ItemSetAbility == null) {
                return TaskStatus.Failure;
            }

            // The ability is not null - try to start the ability.
            return m_CharacterLocomotion.TryStartAbility(m_ItemSetAbility) ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_AbilityType = new ItemSetAbilityString();
            m_CategoryID = new CategoryID();
        }
    }
}
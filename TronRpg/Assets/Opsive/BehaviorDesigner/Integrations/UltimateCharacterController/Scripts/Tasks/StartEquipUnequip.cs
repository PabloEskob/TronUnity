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
    using UnityEngine;

    [NodeDescription("Tries to start the equip unequip ability.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class StartEquipUnequip : TargetGameObjectAction
    {
        [Tooltip("The category that the ability should respond to.")]
        public SharedVariable<CategoryID> m_CategoryID;
        [Tooltip("The ItemSet index that should be equipped or unequipped.")]
        public SharedVariable<int> m_ItemSetIndex;

        private EquipUnequip m_EquipUnequip;

        private GameObject m_PrevTarget;
        private UltimateCharacterLocomotion m_CharacterLocomotion;

        /// <summary>
        /// Retrieves the equip unequip ability.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_CharacterLocomotion = gameObject.GetCachedComponent<UltimateCharacterLocomotion>();
                // Find the specified ability.
                var abilities = m_CharacterLocomotion.GetAbilities<EquipUnequip>();
                // The category ID must match.
                for (int i = 0; i < abilities.Length; ++i) {
                    if (abilities[i].ItemSetCategoryID == m_CategoryID.Value.ID) {
                        m_EquipUnequip = abilities[i];
                        break;
                    }
                }
                if (m_EquipUnequip == null) {
                    // If the EquipUnequip ability can't be found but there is only one EquipUnequip ability added to the character then use that ability.
                    if (abilities.Length == 1) {
                        m_EquipUnequip = abilities[0];
                    } else {
                        Debug.LogWarning($"Error: Unable to find a Equip Unequip ability with category id {m_CategoryID.Value}.");
                        return;
                    }
                }
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Tries to start or stop the use of the current item.
        /// </summary>
        /// <returns>Success if the item was used.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_EquipUnequip == null) {
                return TaskStatus.Failure;
            }

            // The EquipUnequip ability has been found - start the equip or unequip.
            return m_EquipUnequip.StartEquipUnequip(m_ItemSetIndex.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_CategoryID = new CategoryID();
            m_ItemSetIndex = 0;
        }
    }
}
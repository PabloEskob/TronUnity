/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.Shared.Game;
    using Opsive.Shared.Utility;
    using Opsive.UltimateCharacterController.Character;
    using Opsive.UltimateCharacterController.Character.Abilities;
    using UnityEngine;

    [NodeDescription("Is the specified ability active?")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class IsAbilityActive : TargetGameObjectConditional
    {
        [Tooltip("The name of the ability.")]
        public SharedVariable<AbilityString> m_AbilityType;
        [Tooltip("The priority index can be used to specify which ability should be stopped if multiple abilities types are found.")]
        public SharedVariable<int> m_PriorityIndex = -1;

        private GameObject m_PrevTarget;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private Ability m_Ability;

        /// <summary>
        /// Retrieves the specified ability.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_CharacterLocomotion = gameObject.GetCachedComponent<UltimateCharacterLocomotion>();
                // Find the specified ability.
                var abilities = m_CharacterLocomotion.GetAbilities(TypeUtility.GetType(m_AbilityType.Value.Type));
                if (abilities == null) {
                    return;
                }
                if (abilities.Length > 1) {
                    // If there are multiple abilities found then the priority index should be used, otherwise set the ability to the first value.
                    if (m_PriorityIndex.Value != -1) {
                        for (int i = 0; i < abilities.Length; ++i) {
                            if (abilities[i].Index == m_PriorityIndex.Value) {
                                m_Ability = abilities[i];
                                break;
                            }
                        }
                    } else {
                        m_Ability = abilities[0];
                    }
                } else if (abilities.Length == 1) {
                    m_Ability = abilities[0];
                }
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Returns succes if the ability is active.
        /// </summary>
        /// <returns>Success if the ability is active.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Ability == null) {
                return TaskStatus.Failure;
            }

            // The ability is not null - is the ability active?
            return m_Ability.IsActive ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_AbilityType = new AbilityString();
            m_PriorityIndex = -1;
        }
    }
}
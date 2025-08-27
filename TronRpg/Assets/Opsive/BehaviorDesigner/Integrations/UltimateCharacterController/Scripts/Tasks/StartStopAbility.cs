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
    using Opsive.UltimateCharacterController.Character.Abilities;
    using Opsive.Shared.Game;
    using Opsive.Shared.Utility;
    using UnityEngine;

    [NodeDescription("Tries to start or stop the ability.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class StartStopAbility : TargetGameObjectAction
    {
        [Tooltip("The name of the ability to start or stop.")]
        public SharedVariable<AbilityString> m_AbilityType;
        [Tooltip("The priority index can be used to specify which ability should be started or stopped if multiple abilities of the same type are found.")]
        public SharedVariable<int> m_PriorityIndex = -1;
        [Tooltip("Should the ability be started?")]
        public SharedVariable<bool> m_Start = true;
        [Tooltip("Should the task always return success even if the ability doesn't start/stop successfully?")]
        public SharedVariable<bool> m_AlwaysReturnSuccess;

        private GameObject m_PrevTarget;
        protected UltimateCharacterLocomotion m_CharacterLocomotion;
        protected Ability m_Ability;

        /// <summary>
        /// Retrieves the specified ability.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_CharacterLocomotion = gameObject.GetCachedComponent<UltimateCharacterLocomotion>();
                // Find the specified ability.
                var abilities = m_CharacterLocomotion.GetAbilities(TypeUtility.GetType(m_AbilityType.Value.Type));
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
        /// Tries to start or stop the specified ability.
        /// </summary>
        /// <returns>Success if the ability was started or stopped.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Ability == null) {
                return TaskStatus.Failure;
            }

            // The ability is not null - try to start or stop the ability.
            if (m_Start.Value) {
                var abilityStarted = m_CharacterLocomotion.TryStartAbility(m_Ability);
                return (abilityStarted || m_AlwaysReturnSuccess.Value) ? TaskStatus.Success : TaskStatus.Failure;
            } else {
                var abilityStopped = m_CharacterLocomotion.TryStopAbility(m_Ability);
                return (abilityStopped || m_AlwaysReturnSuccess.Value) ? TaskStatus.Success : TaskStatus.Failure;
            }
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_AbilityType = new AbilityString();
            m_PriorityIndex = -1;
            m_Start = true;
            m_AlwaysReturnSuccess = false;
        }
    }
}
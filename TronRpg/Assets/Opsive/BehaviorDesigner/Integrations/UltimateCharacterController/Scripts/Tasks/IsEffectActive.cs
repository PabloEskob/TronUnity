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
    using Opsive.UltimateCharacterController.Character.Effects;
    using UnityEngine;

    [NodeDescription("Is the specified effect active?")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class IsEffectActive : TargetGameObjectConditional
    {
        [Tooltip("The name of the effect.")]
        public SharedVariable<EffectString> m_EffectType;

        private GameObject m_PrevTarget;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private Effect m_Effect;

        /// <summary>
        /// Retrieves the specified effect.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevTarget) {
                m_CharacterLocomotion = gameObject.GetCachedComponent<UltimateCharacterLocomotion>();
                m_Effect = m_CharacterLocomotion.GetEffect(TypeUtility.GetType(m_EffectType.Value.Type));
                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Returns succes if the effect is active.
        /// </summary>
        /// <returns>Success if the effect is active.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Effect == null) {
                return TaskStatus.Failure;
            }

            // The effect is not null - is the effect active?
            return m_Effect.IsActive ? TaskStatus.Success : TaskStatus.Failure;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_EffectType = new EffectString();
        }
    }
}
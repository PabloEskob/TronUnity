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
    using Opsive.UltimateCharacterController.Character.Effects;
    using Opsive.Shared.Game;
    using Opsive.Shared.Utility;
    using UnityEngine;

    [NodeDescription("Tries to start or stop the effect.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class StartStopEffect : TargetGameObjectAction
    {
        [Tooltip("The name of the effect to start or stop.")]
        public SharedVariable<EffectString> m_EffectType;
        [Tooltip("Should the effect be started?")]
        public SharedVariable<bool> m_Start = true;
        [Tooltip("Should the task always return success even if the effect is started/stopped successfully?")]
        public SharedVariable<bool> m_AlwaysReturnSuccess;

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
        /// Tries to start or stop the specified effect.
        /// </summary>
        /// <returns>Success if the effect was started or stopped.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Effect == null) {
                return TaskStatus.Failure;
            }

            // The effect is not null - try to start or stop the effect.
            if (m_Start.Value) {
                var effectStarted = m_CharacterLocomotion.TryStartEffect(m_Effect);
                return (effectStarted || m_AlwaysReturnSuccess.Value) ? TaskStatus.Success : TaskStatus.Failure;
            } else {
                var effectStopped = m_CharacterLocomotion.TryStopEffect(m_Effect);
                return (effectStopped || m_AlwaysReturnSuccess.Value) ? TaskStatus.Success : TaskStatus.Failure;
            }
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_EffectType = new EffectString();
            m_Start = true;
            m_AlwaysReturnSuccess = true;
        }
    }
}
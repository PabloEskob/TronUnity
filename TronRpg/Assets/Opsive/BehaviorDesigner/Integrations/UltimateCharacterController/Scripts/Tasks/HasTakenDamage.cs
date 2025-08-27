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
    using Opsive.Shared.Events;
    using UnityEngine;

    [NodeDescription("Returns success when the agent takes damage.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class HasTakenDamage : TargetGameObjectConditional
    {
        [Tooltip("The GameObject that caused the damage.")]
        public SharedVariable<GameObject> m_Attacker;

        private GameObject m_PrevTarget;
        private int m_DamageFrame = -1;
        private GameObject m_Originator;

        /// <summary>
        /// Retrieves the health component.
        /// </summary>
        public override void OnStart()
        {
            // If the targets aren't equal then the character hasn't been set or the target has switched.
            if (gameObject != m_PrevTarget) {
                if (m_PrevTarget != null) {
                    EventHandler.UnregisterEvent<float, Vector3, Vector3, GameObject, Collider>(m_PrevTarget, "OnHealthDamage", OnDamage);
                    EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_PrevTarget, "OnDeath", OnDeath);
                }

                if (gameObject != null) {
                    EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject, Collider>(gameObject, "OnHealthDamage", OnDamage);
                    EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(gameObject, "OnDeath", OnDeath);
                }

                m_PrevTarget = gameObject;
            }
        }

        /// <summary>
        /// Returns succes if the agent has taken damage.
        /// </summary>
        /// <returns>Success if the agent has taken damage.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_DamageFrame >= Time.frameCount - 1) {
                if (m_Attacker.IsShared) {
                    m_Attacker.Value = m_Originator;
                }
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        /// <summary>
        /// The task has ended - reset the damage variable.
        /// </summary>
        public override void OnEnd()
        {
            m_Originator = null;
            m_DamageFrame = -1;
        }

        /// <summary>
        /// The object has taken damage.
        /// </summary>
        /// <param name="amount">The amount of damage taken.</param>
        /// <param name="position">The position of the damage.</param>
        /// <param name="force">The amount of force applied to the object while taking the damage.</param>
        /// <param name="attacker">The GameObject that did the damage.</param>
        /// <param name="hitCollider">The Collider that was hit.</param>
        private void OnDamage(float amount, Vector3 position, Vector3 force, GameObject attacker, Collider hitCollider)
        {
            m_DamageFrame = Time.frameCount;
            m_Originator = attacker;
        }

        /// <summary>
        /// The object has died.
        /// </summary>
        /// <param name="position">The position of the force.</param>
        /// <param name="force">The amount of force which killed the character.</param>
        /// <param name="attacker">The GameObject that killed the character.</param>
        private void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            m_DamageFrame = -1;
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            m_TargetGameObject = null;
            m_Attacker = null;
        }
    }
}
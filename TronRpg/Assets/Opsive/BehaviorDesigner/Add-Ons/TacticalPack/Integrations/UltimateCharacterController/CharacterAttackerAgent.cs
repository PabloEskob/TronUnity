/// ---------------------------------------------
/// Tactical Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.TacticalPack.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.AddOns.TacticalPack.Runtime;
    using Opsive.UltimateCharacterController.Character;
    using Opsive.UltimateCharacterController.Character.Abilities;
    using Opsive.UltimateCharacterController.Character.Abilities.Items;
    using Opsive.Shared.Game;
    using Opsive.Shared.StateSystem;
    using UnityEngine;

    /// <summary>
    /// Implements IAttackAgent for the Ultimate Character Controller.
    /// </summary>
    public class TacticalCharacterAgent : StateBehavior, IAttackAgent
    {
        [Tooltip("The SlotID of the use ability.")]
        [SerializeField] public int m_SlotID = -1;
        [Tooltip("The ActionID of the use ability.")]
        [SerializeField] public int m_ActionID;
        [Tooltip("The closest distance that the agent is able to attack from.")]
        [SerializeField] protected float m_MinAttackDistance;
        [Tooltip("The furthest distance that the agent is able to attack from.")]
        [SerializeField] protected float m_MaxAttackDistance;
        [Tooltip("The maximum angle from the target to the agent in order for agent to be able to attack.")]
        [SerializeField] protected float m_AttackAngleThreshold;
        [Tooltip("If the target is a humanoid should a bone from the humanoid be targeted?")]
        [SerializeField] protected bool m_TargetHumanoidBone;
        [Tooltip("Specifies which bone to target if targeting a humanoid bone.")]
        [SerializeField] protected HumanBodyBones m_HumanoidBoneTarget = HumanBodyBones.Chest;

        public float MinAttackDistance { get => m_MinAttackDistance; set => m_MinAttackDistance = value; }
        public float MaxAttackDistance { get => m_MaxAttackDistance; set => m_MaxAttackDistance = value; }
        public float AttackAngleThreshold { get => m_AttackAngleThreshold; set => m_AttackAngleThreshold = value; }

        private Transform m_Transform;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private LocalLookSource m_LocalLookSource;
        private Use m_UseAbility;
        private RotateTowards m_RotateTowards;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_Transform = transform;
            m_CharacterLocomotion = GetComponent<UltimateCharacterLocomotion>();
            m_LocalLookSource = GetComponent<LocalLookSource>();
            m_RotateTowards = m_CharacterLocomotion?.GetAbility<RotateTowards>();

            var abilities = m_CharacterLocomotion.GetAbilities<Opsive.UltimateCharacterController.Character.Abilities.Items.Use>();
            // The slot ID and action ID must match.
            for (int i = 0; i < abilities.Length; ++i) {
                if (abilities[i].SlotID == m_SlotID && abilities[i].ActionID == m_ActionID) {
                    m_UseAbility = abilities[i];
                    break;
                }
            }
            if (m_UseAbility == null) {
                // If the Use ability can't be found but there is only one Use ability added to the character then use that ability.
                if (abilities.Length == 1) {
                    m_UseAbility = abilities[0];
                } else {
                    Debug.LogWarning($"Error: Unable to find a Use ability with slot {m_SlotID} and action {m_ActionID}.");
                    return;
                }
            }
        }

        /// <summary>
        /// Rotates the agent towards the specified direction.
        /// </summary>
        /// <param name="direction">The direction to rate towards.</param>
        public void RotateTowards(Vector3 direction)
        {
            return;
            if (m_LocalLookSource.LookTransform != null) {
                var lookTransform = m_LocalLookSource.LookTransform;
                lookTransform.rotation = Quaternion.LookRotation(direction.normalized);
            }
        }

        /// <summary>
        /// Tries to do the actual actual attack.
        /// </summary>
        /// <param name="targetTransform">The target Transform that should be attacked.</param>
        /// <param name="targetDamageable">The target Damageable that should be attacked.</param>
        public void Attack(Transform targetTransform, IDamageable targetDamageable)
        {
            if (m_TargetHumanoidBone) {
                Animator animator = null;
                var modelManager = targetTransform.gameObject.GetCachedComponent<ModelManager>();
                if (modelManager != null) {
                    animator = modelManager.ActiveModel.GetCachedComponent<Animator>();
                } else {
                    var animationMonitor = targetTransform.gameObject.GetComponentInChildren<AnimationMonitorBase>();
                    if (animationMonitor != null) {
                        animator = animationMonitor.gameObject.GetCachedComponent<Animator>();
                    } else {
                        animator = targetTransform.gameObject.GetCachedComponent<Animator>();
                    }
                }
                if (animator != null && animator.isHuman) { 
                    targetTransform = animator.GetBoneTransform(m_HumanoidBoneTarget);
                }
            }

            m_LocalLookSource.Target = targetTransform;
            if (m_RotateTowards != null && targetTransform != null) {
                m_RotateTowards.Target = targetTransform;              // та же цель, что выбрал TacticalBase
                if (!m_RotateTowards.IsActive) {
                    m_CharacterLocomotion.TryStartAbility(m_RotateTowards); ;// корректный API старта способности
                }
            }
            m_CharacterLocomotion.TryStartAbility(m_UseAbility);
        }
    }
}
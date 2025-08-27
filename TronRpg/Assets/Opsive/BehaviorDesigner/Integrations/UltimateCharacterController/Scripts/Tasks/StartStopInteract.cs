/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.UltimateCharacterController.Character.Abilities;
    using Opsive.UltimateCharacterController.Traits;
    using Opsive.Shared.Game;
    using UnityEngine;

    [NodeDescription("Tries to start or stop the Interact ability.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class StartStopInteract : StartStopAbility
    {
        [Tooltip("A reference to the GameObject that the character should interact with.")]
        public SharedVariable<GameObject> m_InteractableGameObject;

        /// <summary>
        /// Retrieves the specified ability.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            var interact = m_Ability as Interact;
            if (interact == null) {
                Debug.LogError("Error: The found ability is not an Interact ability.");
                return;
            }

            if (m_Start.Value) {
                interact.Interactable = m_InteractableGameObject.Value.GetCachedComponent<Interactable>();
                if (interact.Interactable == null) {
                    Debug.LogWarning("Warning: Unable to find the Interactable component on " + m_InteractableGameObject.Value);
                }
            }
        }

        /// <summary>
        /// Resets the objects back to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_InteractableGameObject = null;
        }
    }
}
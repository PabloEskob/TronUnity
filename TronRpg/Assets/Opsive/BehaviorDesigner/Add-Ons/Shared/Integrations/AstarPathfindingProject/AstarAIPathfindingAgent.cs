/// ---------------------------------------------
/// Movement Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.Shared.Integrations.AstarPathfindingProject.Demo
{
    using Opsive.BehaviorDesigner.AddOns.Shared.Demo;
    using Pathfinding;
    using UnityEngine;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// Implements IPathfindingAgent for the IAstarAI agent.
    /// </summary>
    [MovedFrom("Opsive.BehaviorDesigner.AddOns.MovementPack.Integrations.AstarPathfindingProject")]
    public class AstarAIPathfindingAgent : MonoBehaviour, IPathfindingAgent
    {
        private IAstarAI m_AstarAI;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_AstarAI = GetComponent<IAstarAI>();
        }

        /// <summary>
        /// Warps the pathfinding implementation.
        /// </summary>
        /// <param name="position">The target position.</param>
        public void Warp(Vector3 position)
        {
            m_AstarAI.Teleport(position);
        }

        /// <summary>
        /// Sets the target destination.
        /// </summary>
        /// <param name="position">The position that should be set.</param>
        public void SetDestination(Vector3 position)
        {
            m_AstarAI.destination = position;
        }
    }
}
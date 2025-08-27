/// ---------------------------------------------
/// Movement Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.Shared.Integrations.AstarPathfindingProject
{
    using Opsive.BehaviorDesigner.AddOns.Shared.Runtime.Pathfinding;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Pathfinding;
    using UnityEngine;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// Implements the Pathfindiner abstract class for the A* Pathfinding Project implementation.
    /// </summary>
    [MovedFrom("Opsive.BehaviorDesigner.AddOns.MovementPack.Runtime.Tasks")]
    public class AstarAIPathfinder : Pathfinder
    {
        [Tooltip("Should the NavMeshAgent rotation be updated?")]
        [SerializeField] protected SharedVariable<bool> m_UpdateRotation = true;
        [Tooltip("Sets the minimum amount of time in between destination updates. This allows for throttling the number of path searches.")]
        [SerializeField] protected SharedVariable<float> m_DestinationUpdateInterval = 0;

        private IAstarAI m_AstarAI;
        private bool m_StartUpdateRotation;

        public override Vector3 Velocity { get => m_AstarAI.velocity; }
        public override float RemainingDistance { get => m_AstarAI.pathPending ? float.PositiveInfinity : m_AstarAI.remainingDistance; }
        public override float Speed { get => m_AstarAI.maxSpeed; set => m_AstarAI.maxSpeed = value; }
        public override Vector3 Destination { get => m_AstarAI.destination; }

        private float m_SetDestinationTime;

        /// <summary>
        /// Initializes the Pathfinder.
        /// </summary>
        /// <param name="gameObject">The parent GameObject.</param>
        public override void Initialize(GameObject gameObject)
        {
            m_AstarAI = gameObject.GetComponent<IAstarAI>();
            if (m_AstarAI == null) {
                Debug.LogError($"Error: Unable to find the IAstarAI component on the {gameObject} GameObject.");
                return;
            }
        }

        /// <summary>
        /// The task has started.
        /// </summary>
        public override void OnStart()
        {
            m_StartUpdateRotation = m_AstarAI.updateRotation;
            UpdateRotation(m_UpdateRotation.Value);
            m_SetDestinationTime = -m_DestinationUpdateInterval.Value;
        }

        /// <summary>
        /// Specifies if the rotation should be updated.
        /// </summary>
        /// <param name="update">Should the rotation be updated?</param>
        private void UpdateRotation(bool update)
        {
            if (m_AstarAI is FollowerEntity) {
                return;
            }

            m_AstarAI.updateRotation = update;
        }

        /// <summary>
        /// Set a new pathfinding destination.
        /// </summary>
        /// <param name="destination">The destination to set.</param>
        /// <returns>True if the destination is valid.</returns>
        public override bool SetDesination(Vector3 destination)
        {
            if (!m_AstarAI.isStopped && m_AstarAI.destination == destination) {
                return true;
            }

            // Prevent the destination from being set too often.
            if (m_SetDestinationTime + m_DestinationUpdateInterval.Value > Time.time) {
                return true;
            }

            m_AstarAI.isStopped = false;
            m_AstarAI.destination = destination;
            m_AstarAI.SearchPath();
            m_SetDestinationTime = Time.time;
            return true;
        }

        /// <summary>
        /// Does the agent have a pathfinding path?
        /// </summary>
        /// <returns>True if the agent has a pathfinding path.</returns>
        public override bool HasPath()
        {
            return m_AstarAI.hasPath && !m_AstarAI.reachedDestination;
        }

        /// <summary>
        /// Returns true if the position is a valid pathfinding position.
        /// </summary>
        /// <param name="position">The position to sample. The position will be updated to the valid sampled position.</param>
        /// <returns>True if the position is a valid pathfinding position.</returns>
        public override bool SamplePosition(ref Vector3 position)
        {
            if (AstarPath.active == null) {
                return true;
            }
            var nearestNode = AstarPath.active.GetNearest(position).node;
            if (nearestNode.Walkable) {
                position = (Vector3)nearestNode.position;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Has the agent arrived at the destination?
        /// </summary>
        /// <returns>True if the agent has arrived at the destination.</returns>
        public override bool HasArrived()
        {
            return m_AstarAI.reachedDestination;
        }

        /// <summary>
        /// The agent should stop moving.
        /// </summary>
        public override void Stop()
        {
            if (m_AstarAI.isStopped) {
                return;
            }

            m_AstarAI.isStopped = true;
            UpdateRotation(false);
            if (m_AstarAI is FollowerEntity followerEntity && !followerEntity.entityExists) {
                return; // The path can't be set if the entity doesn't exist.
            }
            m_AstarAI.SetPath(null);
        }

        /// <summary>
        /// The task has stopped.
        /// </summary>
        public override void OnEnd()
        {
            UpdateRotation(m_StartUpdateRotation);
        }

        /// <summary>
        /// Resets the values back to their default.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_UpdateRotation = false;
        }
    }
}
using UnityEngine;

namespace Core.Events.Messages
{
    public readonly struct PlayerDodged
    {
    }

    public readonly struct PlayerStartedRunning
    {
    }

    public readonly struct PlayerStoppedRunning
    {
    }

    public readonly struct PlayerMoved
    {
        public readonly Vector3 Direction;
        public PlayerMoved(Vector3 dir) => Direction = dir;
    }

    public readonly struct PlayerStopped
    {
    }

    public readonly struct PlayerFootstep
    {
        public readonly Vector3 Position;

        public PlayerFootstep(Vector3 position) => Position = position;
    }

    public readonly struct PlayerSpawned
    {
        public readonly GameObject Player;
        public PlayerSpawned(GameObject player) => Player = player;
    }

    public struct CombatStarted
    {
    }

    public struct CombatEnded
    {
    }

    public struct AimingStarted
    {
    }

    public struct AimingEnded
    {
    }

    public struct DialogueStarted
    {
        public Transform speaker;
    }

    public struct DialogueEnded
    {
    }

    public struct CutsceneStarted
    {
        public string cutsceneId;
    }

    public struct CutsceneEnded
    {
    }
}
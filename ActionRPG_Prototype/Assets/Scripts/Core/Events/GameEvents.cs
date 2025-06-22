using UnityEngine;
using UnityEngine.Events;

namespace Core.Events
{
    public static class GameEvents
    {
        // Movement Events
        public static readonly UnityEvent<Vector3> OnPlayerMoved = new UnityEvent<Vector3>();
        public static readonly UnityEvent OnPlayerStopped = new UnityEvent();
        public static readonly UnityEvent OnPlayerStartedRunning = new UnityEvent();
        public static readonly UnityEvent OnPlayerStoppedRunning = new UnityEvent();

        // Combat Events
        public static readonly UnityEvent<float> OnPlayerDamaged = new UnityEvent<float>();
        public static readonly UnityEvent OnPlayerDodged = new UnityEvent();
        public static readonly UnityEvent<int> OnPlayerAttacked = new UnityEvent<int>();

        // Camera Events
        public static readonly UnityEvent<string> OnCameraStateChanged = new UnityEvent<string>();
        public static readonly UnityEvent<Transform> OnLockOnTargetChanged = new UnityEvent<Transform>();

        // System Events
        public static readonly UnityEvent OnGamePaused = new UnityEvent();
        public static readonly UnityEvent OnGameResumed = new UnityEvent();
    }
}
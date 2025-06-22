using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Events
{
    public static class GameEvents
    {
        public static event Action<Vector3> PlayerMoved = delegate { };
        public static event Action PlayerStopped = delegate { };
        public static event Action PlayerStartedRunning = delegate { };
        public static event Action PlayerStoppedRunning = delegate { };
        public static event Action<float> PlayerDamaged = delegate { };
        public static event Action PlayerDodged = delegate { };
        public static event Action<int> PlayerAttacked = delegate { };
        public static event Action<string> CameraStateChanged = delegate { };
        public static event Action<Transform> LockOnTargetChanged = delegate { };
        public static event Action GamePaused = delegate { };
        public static event Action GameResumed = delegate { };

        public static void InvokePlayerMoved(Vector3 v) => PlayerMoved.Invoke(v);
        public static void InvokePlayerStopped() => PlayerStopped.Invoke();
        public static void InvokePlayerStartedRunning() => PlayerStartedRunning.Invoke();
        public static void InvokePlayerStoppedRunning() => PlayerStoppedRunning.Invoke();
        public static void InvokePlayerDamaged(float dmg) => PlayerDamaged.Invoke(dmg);
        public static void InvokePlayerDodged() => PlayerDodged.Invoke();
        public static void InvokePlayerAttacked(int idx) => PlayerAttacked.Invoke(idx);
        public static void InvokeCameraStateChanged(string s) => CameraStateChanged.Invoke(s);
        public static void InvokeLockOnTargetChanged(Transform t) => LockOnTargetChanged.Invoke(t);
        public static void InvokeGamePaused() => GamePaused.Invoke();
        public static void InvokeGameResumed() => GameResumed.Invoke();
    }
}
using Pathfinding;
using UnityEngine;

namespace Core.Scripts.Logic
{
    public class AIDebugHUD : MonoBehaviour
    {
        public IAstarAI ai;
        void Awake() => ai = GetComponent<IAstarAI>();

        void OnGUI()
        {
            if (ai == null) return;
            GUILayout.Label($"hasPath={ai.hasPath}  pending={ai.pathPending}");
            GUILayout.Label($"reachedDest={ai.reachedDestination}  reachedEnd={ai.reachedEndOfPath}");
            GUILayout.Label($"remDist={ai.remainingDistance:F2}");
            GUILayout.Label($"isStopped={ai.isStopped}  canMove={ai.canMove}  canSearch={ai.canSearch}");
        }
    }
}
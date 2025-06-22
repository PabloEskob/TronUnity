using UnityEngine;

namespace Core.Utils
{
    public static class ComponentFinder
    {
        // === Single ===
        public static bool TryFind<T>(this GameObject go, out T result, bool includeInactive = true)
            where T : Component
        {
            result = go.GetComponentInChildren<T>(includeInactive);
#if UNITY_EDITOR
            if (result == null)
                Debug.LogWarning($"[ComponentFinder] {typeof(T).Name} not found under {go.name}", go);
#endif
            return result;
        }

        public static T Require<T>(this GameObject go, bool includeInactive = true) where T : Component
        {
            if (!go.TryFind(out T comp, includeInactive))
                throw new MissingComponentException($"{typeof(T).Name} required on {go.name}");
            return comp;
        }

        // === Multiple ===
        public static int FindAll<T>(this GameObject go, ref T[] buffer, bool includeInactive = true)
            where T : Component
        {
            buffer = go.GetComponentsInChildren<T>(includeInactive);
            return buffer.Length;
        }
    }
}
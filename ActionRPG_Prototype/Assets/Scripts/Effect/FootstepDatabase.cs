using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(menuName = "Pavel/Footsteps/Database", fileName = "FootstepDatabase")]
    public sealed class FootstepDatabase : ScriptableObject
    {
        [SerializeField] private FootstepProfile _defaultProfile;
        [SerializeField] private List<FootstepProfile> _profiles;

        private Dictionary<PhysicsMaterial, FootstepProfile> _byMat;
        private Dictionary<string, FootstepProfile> _byTag;

        public FootstepProfile DefaultProfile => _defaultProfile;

        private void OnEnable()
        {
            _byMat = new();
            _byTag = new();
            foreach (var p in _profiles)
            {
                if (p.physicMaterial != null) _byMat[p.physicMaterial] = p;
                if (!string.IsNullOrEmpty(p.surfaceTag)) _byTag[p.surfaceTag] = p;
            }
        }

        public FootstepProfile Resolve(RaycastHit hit)
        {
            if (hit.collider == null) return _defaultProfile;

            var mat = hit.collider.sharedMaterial;
            if (mat != null && _byMat.TryGetValue(mat, out var prof)) return prof;

            var tag = hit.collider.tag;
            if (_byTag.TryGetValue(tag, out prof)) return prof;

            return _defaultProfile;
        }
    }
}
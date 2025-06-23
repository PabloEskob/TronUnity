using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    public static class FootstepPool
    {
        private static readonly Queue<AudioSource> _audioPool = new();
        private static readonly Dictionary<GameObject, Queue<ParticleSystem>> _fxPool = new();

        public static AudioSource GetAudio()
        {
            if (_audioPool.Count > 0) return _audioPool.Dequeue();
            var go = new GameObject("[FootstepAudio]");
            Object.DontDestroyOnLoad(go);
            var src = go.AddComponent<AudioSource>();
            src.spatialBlend = 1;
            return src;
        }

        public static void Recycle(AudioSource src) => _audioPool.Enqueue(src);

        public static ParticleSystem GetFX(GameObject prefab)
        {
            if (!_fxPool.TryGetValue(prefab, out var q))
                _fxPool[prefab] = q = new();

            if (q.Count > 0) return q.Dequeue();
            return Object.Instantiate(prefab).GetComponent<ParticleSystem>();
        }

        public static void Recycle(GameObject prefab, ParticleSystem fx)
        {
            fx.Stop();
            _fxPool[prefab].Enqueue(fx);
        }
    }
}
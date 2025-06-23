using Character.Animation;
using Core.Events;
using UnityEngine;

namespace Effect
{
    [AddComponentMenu("Pavel/FX/Footstep Emitter")]
    public sealed class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] private FootstepDatabase _db;
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private float _rayDistance = 1.4f;

        private Transform _tf;

        private void Awake()
        {
            _tf = transform;
            // Подписка на глобальное UnityEvent – через инспектор либо код:
            if (TryGetComponent(out AnimationEventHandler ev))
                ev.OnFootstep();
        }

        private void OnAnimEvent(string evt)
        {
            if (evt == "FootstepL" || evt == "FootstepR")
                EmitFootstep();
        }

        public void EmitFootstep()
        {
            if (!Physics.Raycast(_tf.position + Vector3.up * 0.2f, Vector3.down,
                    out var hit, _rayDistance, _groundMask, QueryTriggerInteraction.Ignore))
                return;

            var profile = _db.Resolve(hit);

            // 1. Audio
            if (profile.clips.Length > 0)
            {
                var clip = profile.clips[Random.Range(0, profile.clips.Length)];
                var src = FootstepPool.GetAudio();
                src.transform.position = hit.point;
                src.PlayOneShot(clip);
                _ = DelayedRecycle(src, clip.length);
            }

            // 2. Dust FX
            if (profile.createDust && profile.dustPrefab != null)
            {
                var fx = FootstepPool.GetFX(profile.dustPrefab);
                fx.transform.SetPositionAndRotation(hit.point + hit.normal * 0.02f,
                    Quaternion.LookRotation(hit.normal));
                fx.Play();
                _ = DelayedRecycle(profile.dustPrefab, fx, fx.main.duration);
            }

            // 3. Footprint
            if (profile.footprintPrefab)
            {
                Object.Instantiate(profile.footprintPrefab,
                    hit.point + hit.normal * 0.01f,
                    Quaternion.FromToRotation(Vector3.up, hit.normal));
            }

            // 4. Optional global event (for sound occlusion / analytics)
            GameEvents.InvokePlayerMoved(Vector3.zero); // placeholder
        }

        // ───── helpers ─────
        private static async System.Threading.Tasks.Task DelayedRecycle(AudioSource src, float delay)
        {
            await System.Threading.Tasks.Task.Delay((int)(delay * 1000));
            FootstepPool.Recycle(src);
        }

        private static async System.Threading.Tasks.Task DelayedRecycle(GameObject prefab, ParticleSystem fx, float delay)
        {
            await System.Threading.Tasks.Task.Delay((int)(delay * 1000));
            FootstepPool.Recycle(prefab, fx);
        }
    }
}
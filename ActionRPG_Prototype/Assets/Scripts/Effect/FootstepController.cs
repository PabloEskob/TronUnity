// FootstepController.cs

using Character;
using UnityEngine;

namespace Effect
{
    public class FootstepController : MonoBehaviour
    {
        [Header("Footstep Settings")] [SerializeField]
        private AudioSource _audioSource;

        [SerializeField] private ParticleSystem _dustEffect;

        [Header("Surface Types")] [SerializeField]
        private FootstepProfile _defaultProfile;

        [SerializeField] private FootstepProfile[] _surfaceProfiles;

        private CharacterAnimationController _animationController;

        private void Awake()
        {
            _animationController = GetComponentInParent<CharacterAnimationController>();
            if (_animationController != null)
            {
                _animationController.OnFootstep += PlayFootstep;
            }
        }

        private void PlayFootstep(int foot)
        {
            // Определяем тип поверхности
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            {
                FootstepProfile profile = GetProfileForSurface(hit.collider);

                // Воспроизводим звук
                if (profile.footstepSounds.Length > 0)
                {
                    var randomClip = profile.footstepSounds[Random.Range(0, profile.footstepSounds.Length)];
                    _audioSource.PlayOneShot(randomClip);
                }

                // Создаем эффект
                if (_dustEffect != null && profile.createDust)
                {
                    _dustEffect.transform.position = hit.point;
                    _dustEffect.Play();
                }
            }
        }

        private FootstepProfile GetProfileForSurface(Collider surface)
        {
            // Логика определения типа поверхности
            return _defaultProfile;
        }
    }

    [System.Serializable]
    public class FootstepProfile
    {
        public string surfaceName;
        public AudioClip[] footstepSounds;
        public bool createDust;
        public GameObject footprintPrefab;
    }
}
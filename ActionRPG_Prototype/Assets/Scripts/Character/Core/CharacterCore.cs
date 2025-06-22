using Character.Animation;
using Character.Combat;
using Character.Movement;
using Character.Stats;
using Core.Utils;
using UnityEngine;

namespace Character.Core
{
    /// <summary>
    /// Central hub that wires together all sub‑systems attached to a character.
    /// Теперь использует утилиту <see cref="ComponentFinder.Require{T}"/>, поэтому
    /// проверка зависимостей проще и чище.
    /// </summary>
    [AddComponentMenu("Character/Character Core")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterCore : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private CharacterConfig _config;

        // ───── Public API ─────
        public CharacterConfig Config { get; private set; }
        public MovementController Movement { get; private set; }
        public MovementStateMachine MovementSM { get; private set; }
        public CombatController Combat { get; private set; }
        public AnimationController Animation { get; private set; }
        public CharacterStats Stats { get; private set; }
        public HealthSystem Health { get; private set; }

        public bool IsPlayer { get; private set; }

        private void Awake()
        {
            CacheDependencies();
        }

        private void Start() => InitializeCharacter();

        private void CacheDependencies()
        {
            Config = _config;
            Movement = gameObject.Require<MovementController>();
            MovementSM = gameObject.Require<MovementStateMachine>();
            Combat = gameObject.Require<CombatController>();
            Animation = gameObject.Require<AnimationController>();
            Stats = gameObject.Require<CharacterStats>();
            Health = gameObject.Require<HealthSystem>();
        }

        private void InitializeCharacter()
        {
            if (Config == null)
            {
                Debug.LogError($"{name}: Config is null", this);
                enabled = false;
                return;
            }

            Stats.InitializeFromConfig(Config);
            Health.InitializeFromConfig(Config);

            IsPlayer = CompareTag("Player");
            if (IsPlayer) SetupPlayerCamera();
        }

        /// <summary>
        /// Подключает Cinemachine‑камеру либо, для старых сцен, CameraController.
        /// </summary>
        private void SetupPlayerCamera()
        {
            // 1) Новый вариант – Cinemachine rig
            var cmRig = FindFirstObjectByType<CameraModeSwitcher>();
            if (cmRig != null)
            {
                cmRig.SetMode(CameraMode.Free);
            }
        }
    }
}
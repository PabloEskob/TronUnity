using Character.Animation;
using Character.Movement;
using Character.Stats;
using Core.Camera;
using Core.Services;
using UnityEngine;

namespace Character.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterCore : MonoBehaviour
    {
        [Header("Core Components")] [SerializeField]
        private CharacterConfig _config;

        // Cached components
        private MovementController _movement;
        private MovementStateMachine _movementStateMachine;
        private CombatController _combat;
        private AnimationController _animation;
        private CharacterStats _stats;
        private HealthSystem _health;

        // Properties
        public CharacterConfig Config => _config;
        public MovementController Movement => _movement;
        public MovementStateMachine MovementStateMachine => _movementStateMachine;
        public CombatController Combat => _combat;
        public AnimationController Animation => _animation;
        public CharacterStats Stats => _stats;
        public HealthSystem Health => _health;

        public bool IsPlayer { get; private set; }

        private void Awake()
        {
            CacheComponents();
            ValidateComponents();
        }

        private void Start()
        {
            InitializeCharacter();
        }

        private void CacheComponents()
        {
            _movement = ComponentFinder.FindFirstInChildren<MovementController>(gameObject);
            _movementStateMachine = ComponentFinder.FindFirstInChildren<MovementStateMachine>(gameObject);
            _combat = ComponentFinder.FindFirstInChildren<CombatController>(gameObject);
            _animation = ComponentFinder.FindFirstInChildren<AnimationController>(gameObject);
            _stats = ComponentFinder.FindFirstInChildren<CharacterStats>(gameObject);
            _health = ComponentFinder.FindFirstInChildren<HealthSystem>(gameObject);
        }

        private void ValidateComponents()
        {
            if (_config == null)
            {
                UnityEngine.Debug.LogError($"CharacterConfig is missing on {gameObject.name}!");
            }

            // Validate other required components
            var requiredComponents = new System.Type[]
            {
                typeof(MovementController),
                typeof(MovementStateMachine),
                typeof(CombatController),
                typeof(AnimationController),
                typeof(CharacterStats),
                typeof(HealthSystem)
            };

            foreach (var type in requiredComponents)
            {
                var method = typeof(ComponentFinder).GetMethod(nameof(ComponentFinder.FindFirstInChildren))
                    ?.MakeGenericMethod(type);
    
                var found = method?.Invoke(null, new object[] { gameObject, false });
    
                if (found == null)
                {
                    UnityEngine.Debug.LogError($"{type.Name} is missing on {gameObject.name} or its children!");
                }
            }
        }

        private void InitializeCharacter()
        {
            // Apply configuration
            if (_config != null)
            {
                _stats.InitializeFromConfig(_config);
                _health.InitializeFromConfig(_config);
            }

            // Setup based on character type
            IsPlayer = CompareTag("Player");

            if (IsPlayer)
            {
                SetupPlayerCharacter();
            }
            else
            {
                SetupAICharacter();
            }
        }

        private void SetupPlayerCharacter()
        {
            // Player-specific setup
            var cameraController = FindFirstObjectByType<CameraController>();
            if (cameraController != null)
            {
                var stateManager = cameraController.GetComponent<CameraStateManager>();
                stateManager.Target = transform;
            }
        }

        private void SetupAICharacter()
        {
            // AI-specific setup
            // Add AI controller initialization here
        }
    }
}
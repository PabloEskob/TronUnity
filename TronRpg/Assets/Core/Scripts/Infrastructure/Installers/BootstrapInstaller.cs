using Core.Scripts.AssetManagement;
using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.Factory;
using Core.Scripts.Infrastructure.States.GameStates;
using Core.Scripts.Infrastructure.States.StateMachine;
using Core.Scripts.Logic;
using Core.Scripts.Services.Input;
using Core.Scripts.Services.PersistentProgress;
using Core.Scripts.Services.SaveLoad;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Infrastructure.Installers
{
    public class BootstrapInstaller : LifetimeScope, IStartable
    {
        [Header("System Prefabs")] [SerializeField]
        private GameObject SystemComponentsPrefab;

        private GameObject _systemComponentsInstance;
        private CoroutineRunner _coroutineRunner;
        private LoadingCurtain _loadingCurtain;

        protected override void Configure(IContainerBuilder builder)
        {
            CreateSystemComponents();

            BindInputService(builder);
            BindAssetManagementServices(builder);
            BindCommonServices(builder);
            BindStateMachine(builder);
            BindStateFactory(builder);
            BindGameStates(builder);
            BindProgressService(builder);
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }

        private void CreateSystemComponents()
        {
            if (SystemComponentsPrefab != null)
            {
                _systemComponentsInstance = Instantiate(SystemComponentsPrefab);
                DontDestroyOnLoad(_systemComponentsInstance);

                _coroutineRunner = _systemComponentsInstance.GetComponent<CoroutineRunner>();
                _loadingCurtain = _systemComponentsInstance.GetComponent<LoadingCurtain>();

                if (_coroutineRunner == null)
                    Debug.LogError("CoroutineRunner not found on SystemComponentsPrefab!");
                if (_loadingCurtain == null)
                    Debug.LogError("LoadingCurtain not found on SystemComponentsPrefab!");
            }
            else
            {
                Debug.LogError("SystemComponentsPrefab is not assigned!");
            }
        }

        private void BindCommonServices(IContainerBuilder builder)
        {
            if (_coroutineRunner != null)
                builder.RegisterComponent(_coroutineRunner).AsImplementedInterfaces();

            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);

            if (_loadingCurtain != null)
                builder.RegisterComponent(_loadingCurtain);
        }

        private void BindStateMachine(IContainerBuilder builder)
        {
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private void BindStateFactory(IContainerBuilder builder)
        {
            builder.Register<IGameFactory, GameFactory>(Lifetime.Singleton);
        }

        private void BindGameStates(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Singleton);
            builder.Register<LoadLevelState>(Lifetime.Singleton);
            builder.Register<GameLoopState>(Lifetime.Singleton);
            builder.Register<LoadProgressState>(Lifetime.Singleton);
        }

        private void BindProgressService(IContainerBuilder builder)
        {
            builder.Register<IPersistentProgressService, PersistentProgressService>(Lifetime.Singleton);
            builder.Register<ISaveLoadService, SaveLoadService>(Lifetime.Singleton);
        }

        private void BindAssetManagementServices(IContainerBuilder builder)
        {
            builder.Register<IAssetProvider, AssetProvider>(Lifetime.Singleton);
        }

        private void BindInputService(IContainerBuilder builder)
        {
            builder.Register<GameInput>(Lifetime.Singleton);
            builder.Register<IInputService, InputService>(Lifetime.Singleton);
        }
    }
}
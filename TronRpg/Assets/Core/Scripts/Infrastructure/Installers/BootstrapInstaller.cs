using Core.Scripts.AssetManagement;
using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.Factory;
using Core.Scripts.Infrastructure.States.GameStates;
using Core.Scripts.Infrastructure.States.StateMachine;
using Core.Scripts.Logic;
using Core.Scripts.Services.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Infrastructure.Installers
{
    public class BootstrapInstaller : LifetimeScope, IStartable
    {
        [SerializeField] 
        private CoroutineRunner _coroutineRunner;
        public LoadingCurtain LoadingCurtain;

        protected override void Configure(IContainerBuilder builder)
        {
            BindInputService(builder);
            BindAssetManagementServices(builder);
            BindCommonServices(builder);
            BindCameraProvider(builder);
            BindStateMachine(builder);
            BindStateFactory(builder);
            BindGameStates(builder);
        }
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject); // Добавьте эту строку
        }

        private void BindCommonServices(IContainerBuilder builder)
        {
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
            
            if (LoadingCurtain != null)
                builder.RegisterComponent(LoadingCurtain);
            
            if (_coroutineRunner != null)
                builder.RegisterComponent(_coroutineRunner).AsImplementedInterfaces();
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
        }


        private void BindCameraProvider(IContainerBuilder builder)
        {
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

        public void Start()
        {
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }
    }
}
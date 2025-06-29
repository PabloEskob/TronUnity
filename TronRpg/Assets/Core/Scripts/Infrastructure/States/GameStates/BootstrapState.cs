using Core.Scripts.Infrastructure.Installers;
using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.StateInfrastructure;
using Core.Scripts.Infrastructure.States.StateMachine;
using Core.Scripts.Services.Input;

namespace Core.Scripts.Infrastructure.States.GameStates
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;


        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            RegisterServices();
            _sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadLevelState, string>("Main");

        private void RegisterServices()
        {
            Game.InputService = RegisterInputService();
        }

        public void Exit()
        {
        }

        private static IInputService RegisterInputService()
        {
            return new InputService(new GameInput());
        }
    }
}
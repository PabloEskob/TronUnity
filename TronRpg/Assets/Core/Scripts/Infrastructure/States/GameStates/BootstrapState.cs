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
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;


        public BootstrapState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadLevelState, string>("Main");
        

        public void Exit()
        {
        }
    }
}
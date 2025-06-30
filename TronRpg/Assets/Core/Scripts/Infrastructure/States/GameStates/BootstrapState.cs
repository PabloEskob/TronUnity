using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.StateInfrastructure;
using Core.Scripts.Infrastructure.States.StateMachine;

namespace Core.Scripts.Infrastructure.States.GameStates
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;


        public BootstrapState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.LoadScene(Scenes.Initial, onLoaded: EnterLoadLevel);
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadProgressState>();


        public void Exit()
        {
        }
    }
}
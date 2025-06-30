using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.Factory;
using Core.Scripts.Infrastructure.States.StateInfrastructure;
using Core.Scripts.Infrastructure.States.StateMachine;
using Core.Scripts.Logic;
using UnityEngine;

namespace Core.Scripts.Infrastructure.States.GameStates
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";

        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly IGameFactory _gameFactory;

        public LoadLevelState(IGameStateMachine gameStateMachine, ISceneLoader sceneLoader, LoadingCurtain loadingCurtain, IGameFactory gameFactory)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _gameFactory = gameFactory;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();
            _gameFactory.Cleanup();
            _sceneLoader.LoadScene(sceneName, onLoaded: OnLoaded);
        }

        public void Exit()
        {
            _loadingCurtain.Hide();
        }

        private void OnLoaded()
        {
            var hero = _gameFactory.CreateHero(GameObject.FindGameObjectWithTag(InitialPointTag));
            _gameFactory.CreateCamera(hero);

            _gameStateMachine.Enter<GameLoopState>();
        }
    }
}
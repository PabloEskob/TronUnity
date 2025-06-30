using Core.Scripts.Data;
using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.StateInfrastructure;
using Core.Scripts.Infrastructure.States.StateMachine;
using Core.Scripts.Services.PersistentProgress;
using Core.Scripts.Services.SaveLoad;

namespace Core.Scripts.Infrastructure.States.GameStates
{
    public class LoadProgressState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;

        public LoadProgressState(IGameStateMachine gameStateMachine, IPersistentProgressService progressService, ISaveLoadService saveLoadService)
        {
            _gameStateMachine = gameStateMachine;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }

        public void Enter()
        {
            LoadProgressOrInitNew();
            _gameStateMachine.Enter<LoadLevelState, string>(_progressService.Progress.WorldData.PositionOnLevel.Level);
        }

        public void Exit()
        {
        }

        private void LoadProgressOrInitNew()
        {
            _progressService.Progress = _saveLoadService.LoadProgress() ?? NewProgress();
        }

        private PlayerProgress NewProgress() =>
            new(Scenes.Main);
    }
}
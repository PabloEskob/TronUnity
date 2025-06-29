using Core.Scripts.Infrastructure.Loading;
using Core.Scripts.Infrastructure.States.StateMachine;
using Core.Scripts.Logic;
using Core.Scripts.Services.Input;

namespace Core.Scripts.Infrastructure.Installers
{
    public class Game
    {
        public readonly GameStateMachine StateMachine;
        public static IInputService InputService;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain loadingCurtain)
        {
            StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), loadingCurtain);
        }
    }
}
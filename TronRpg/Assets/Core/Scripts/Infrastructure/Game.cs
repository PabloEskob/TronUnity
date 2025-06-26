using Core.Scripts.Services.Input;

namespace Core.Scripts.Infrastructure
{
    public class Game
    {
        public static IInputService InputService;

        public Game()
        {
            InputService = new InputService(new GameInput());
        }
    }
}
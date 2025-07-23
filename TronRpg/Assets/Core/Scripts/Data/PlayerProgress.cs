using System;

namespace Core.Scripts.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public State HeroState;
        public WorldData WorldData;

        public PlayerProgress(string initialLevel)
        {
            WorldData = new WorldData(initialLevel);
            HeroState = new State();
        }
    }
}
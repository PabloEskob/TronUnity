using Core.Scripts.Infrastructure.States.Factory;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Character.Enemy
{
    public abstract class Follow: MonoBehaviour
    {
        protected bool _isPursuing;
        
        [Inject] protected IGameFactory _gameFactory;

        public void StopPursuit()
        {
            _isPursuing = false;
            Debug.Log($"{name}: Pursuit stopped.");
        }

        public void ResumePursuit()
        {
            _isPursuing = true;
            Debug.Log($"{name}: Pursuit resumed.");
        }
    }
}
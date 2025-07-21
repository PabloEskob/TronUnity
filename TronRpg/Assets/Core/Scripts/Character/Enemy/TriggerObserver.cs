using System;
using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    [RequireComponent(typeof(Collider))]
    public class TriggerObserver : MonoBehaviour
    {
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other != null)
                TriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other != null)
                TriggerExit?.Invoke(other);
        }
        
        private void OnDestroy()
        {
            TriggerEnter = null;
            TriggerExit = null;
        }
    }
}
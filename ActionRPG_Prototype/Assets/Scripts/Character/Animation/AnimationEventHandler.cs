using UnityEngine;
using UnityEngine.Events;

namespace Character.Animation
{
    public class AnimationEventHandler : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationEvent
        {
            public string eventName;
            public UnityEvent onTrigger;
        }

        [SerializeField] private AnimationEvent[] _events;

        private void TriggerEvent(string eventName)
        {
            foreach (var animEvent in _events)
            {
                if (animEvent.eventName == eventName)
                {
                    animEvent.onTrigger?.Invoke();
                    break;
                }
            }
        }

        // Called from animation events
        public void OnFootstep()
        {
            TriggerEvent("Footstep");
        }

        public void OnAttackStart()
        {
            TriggerEvent("AttackStart");
        }

        public void OnAttackHit()
        {
            TriggerEvent("AttackHit");
        }

        public void OnAttackEnd()
        {
            TriggerEvent("AttackEnd");
        }

        public void OnDodgeStart()
        {
            TriggerEvent("DodgeStart");
        }

        public void OnDodgeEnd()
        {
            TriggerEvent("DodgeEnd");
        }
    }
}
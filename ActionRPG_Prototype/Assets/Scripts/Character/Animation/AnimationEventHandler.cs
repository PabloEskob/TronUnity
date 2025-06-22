using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Character.Animation
{
    [AddComponentMenu("Character/Animation Event Handler")]
    [DisallowMultipleComponent]
    public sealed class AnimationEventHandler : MonoBehaviour
    {
        [System.Serializable]
        public sealed class AnimationEvent { public string Name = "Event"; public UnityEvent OnTrigger; }

        [SerializeField] private AnimationEvent[] _events;
        private Dictionary<string, UnityEvent>    _lookup;

        private void Awake()
        {
            _lookup = new(_events.Length);
            foreach (var e in _events)
            {
                if (string.IsNullOrWhiteSpace(e.Name)) continue;
                _lookup[e.Name] = e.OnTrigger;
            }
        }

        public void InvokeEvent(string n) { if (_lookup.TryGetValue(n, out var u)) u?.Invoke(); }

        // Animation Event proxies
        public void OnFootstep()    => InvokeEvent("Footstep");
        public void OnAttackStart() => InvokeEvent("AttackStart");
        public void OnAttackHit()   => InvokeEvent("AttackHit");
        public void OnAttackEnd()   => InvokeEvent("AttackEnd");
        public void OnDodgeStart()  => InvokeEvent("DodgeStart");
        public void OnDodgeEnd()    => InvokeEvent("DodgeEnd");
    }
}
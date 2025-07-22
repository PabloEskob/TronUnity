using System.Collections;
using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class Aggro : MonoBehaviour
    {
        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private FollowerEntityAdapter _follow;
        [SerializeField] private Follow _moveToPlayer;
        [SerializeField] private float _cooldown;

        private Coroutine _aggroCoroutine;
        private bool _hasAggroTarget;

        private void Start()
        {
            _triggerObserver.TriggerEnter += TriggerEnter;
            _triggerObserver.TriggerExit += TriggerExit;

            SwitchFollowOff();
        }

        private void TriggerEnter(Collider obj)
        {
            if (!_hasAggroTarget)
            {
                _hasAggroTarget = true;
                StopAgroCoroutine();
                SwitchFollowOn();
            }
        }

        private void TriggerExit(Collider obj)
        {
            if (_hasAggroTarget)
            {
                _hasAggroTarget = false;
                _aggroCoroutine = StartCoroutine(SwitchFollowOffAfterCooldown());
            }
        }

        private IEnumerator SwitchFollowOffAfterCooldown()
        {
            yield return new WaitForSeconds(_cooldown);

            SwitchFollowOff();
        }

        private void StopAgroCoroutine()
        {
            if (_aggroCoroutine != null)
            {
                StopCoroutine(_aggroCoroutine);
                _aggroCoroutine = null;
            }
        }

        private void SwitchFollowOff()
        {
            _moveToPlayer.StopPursuit();
            _follow.StopMovement();
        }

        private void SwitchFollowOn()
        {
            _moveToPlayer.ResumePursuit();
            _follow.ResumeMovement();
        }

        private void OnDestroy()
        {
            _triggerObserver.TriggerEnter -= TriggerEnter;
            _triggerObserver.TriggerExit -= TriggerExit;
        }
    }
}
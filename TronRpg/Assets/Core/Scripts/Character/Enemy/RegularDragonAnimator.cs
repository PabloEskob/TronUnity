using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    public class RegularDragonAnimator : BaseEnemyAnimator
    {
        [SerializeField] private GameObject _dragonModel;
        
        protected override void InitializeSpecific()
        {
            _dragonModel.SetActive(true);
            CurrentState = EnemyState.Idle;
        }
    }
}
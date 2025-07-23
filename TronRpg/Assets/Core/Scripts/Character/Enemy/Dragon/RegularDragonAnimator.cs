using UnityEngine;

namespace Core.Scripts.Character.Enemy
{
    /// <summary>
    /// Конкретная реализация аниматора для дракона.
    /// Активирует модель дракона и инициализирует состояние Idle.
    /// </summary>
    public class RegularDragonAnimator : BaseEnemyAnimator
    {
        [SerializeField] private GameObject _dragonModel;

        protected override void InitializeSpecific()
        {
            if (_dragonModel == null)
            {
                Debug.LogError("DragonModel is not assigned in RegularDragonAnimator!");
                enabled = false;
                return;
            }
            _dragonModel.SetActive(true);
            CurrentState = EnemyState.Idle;
        }
    }
}
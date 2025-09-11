using Unity.Cinemachine;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace Core.Scripts.UI
{
    public class BlockCameraUIPanels : MonoBehaviour
    {
        [SerializeField] private CinemachineInputAxisController cmInput;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (!cmInput) cmInput = FindFirstObjectByType<CinemachineInputAxisController>();
        }

        void OnEnable()
        {
            EventHandler.RegisterEvent<bool>(gameObject, "OnEnableGameplayInput", OnGameplayInput);
        }

        void OnDisable()
        {
            EventHandler.UnregisterEvent<bool>(gameObject, "OnEnableGameplayInput", OnGameplayInput);
        }

        private void OnGameplayInput(bool enable)
        {
            if (cmInput) cmInput.enabled = enable; // false при открытии Chest/Inventory/Shop, true при закрытии
        }
    }
}
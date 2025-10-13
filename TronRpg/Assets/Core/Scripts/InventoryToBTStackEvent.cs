using Opsive.BehaviorDesigner.Runtime;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.Shared.Events;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using UnityEngine;

public class InventoryToBTStackEvent : MonoBehaviour
{
    [SerializeField] private string m_ItemDefinitionName = "Log";
    [SerializeField] private int m_StackLimit = 20;

    private Inventory m_Inventory;
    private ItemDefinition m_Def;
    private BehaviorTree m_BT;

    private void Awake()
    {
        m_Inventory = GetComponentInParent<Inventory>();
        m_BT = GetComponentInParent<BehaviorTree>();
        if (!string.IsNullOrEmpty(m_ItemDefinitionName))
        {
            m_Def = InventorySystemManager.GetItemDefinition(m_ItemDefinitionName);
        }
    }

    private void OnEnable()
    {
        if (m_Inventory == null) return;
        EventHandler.RegisterEvent<ItemInfo, ItemStack>(
            m_Inventory,
            EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack, // UIS: событие добавления
            OnAddedItem);
    }

    private void OnDisable()
    {
        if (m_Inventory == null) return;
        EventHandler.UnregisterEvent<ItemInfo, ItemStack>(
            m_Inventory,
            EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack,
            OnAddedItem);
    }

    private void OnAddedItem(ItemInfo added, ItemStack stack)
    {
        if (m_BT == null || stack == null || stack.Item == null || m_Def == null) return;
        // Это наш ресурс?
        if (!m_Def.InherentlyContains(stack.Item)) return;
        // Стек достиг лимита?
        if (stack.Amount >= m_StackLimit)
        {
            EventHandler.ExecuteEvent(m_BT, "StackFull");
        }
    }
}
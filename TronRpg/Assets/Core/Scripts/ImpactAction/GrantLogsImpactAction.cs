using System;
using Opsive.UltimateCharacterController.Items.Actions.Impact;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;

[Serializable]
public class GrantLogsImpactAction : ImpactAction
{
    public string LogItemDefinitionName = "Log";
    public int AmountPerHit = 1;
    public string TargetTag = "Tree";

    [NonSerialized] private ItemDefinition m_LogDefinition;

    protected override void OnImpactInternal(ImpactCallbackContext ctx)
    {
        var data = ctx?.ImpactCollisionData;
        if (data == null) return;

        // 1) Фильтруем цель по тегу.
        var targetGO = data.ImpactGameObject;
        if (targetGO == null) return;
        if (!string.IsNullOrEmpty(TargetTag) && !targetGO.CompareTag(TargetTag)) return;

        // 2) Определяем ИСТОЧНИК удара (дровосек).
        GameObject sourceCharacter =
            (data.SourceCharacterLocomotion != null ? data.SourceCharacterLocomotion.gameObject : null)
            ?? data.SourceGameObject;   // оба поля есть в вашей версии

        if (sourceCharacter == null) return;
        
        var inventory = sourceCharacter.GetComponentInParent<Inventory>();
        if (inventory == null) return;

        inventory.AddItem(LogItemDefinitionName, AmountPerHit);
        // Иначе — рюкзак/коллекция не может принять предмет: можно послать событие в BD.
    }
}
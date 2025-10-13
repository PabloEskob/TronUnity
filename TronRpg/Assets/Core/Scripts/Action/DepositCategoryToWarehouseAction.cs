using UnityEngine;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;

public class DepositCategoryToWarehouseAction : Action
{
    [Tooltip("GO склада (на нём есть Inventory).")]
    public SharedVariable<GameObject> Warehouse;

    [Tooltip("Имя коллекции склада (пусто — MainItemCollection).")]
    public SharedVariable<string> WarehouseCollectionName;

    [Tooltip("Имя категории, которую сдаём (напр., \"Resources\").")]
    public SharedVariable<string> CategoryName = "Resources";

    [Tooltip("Переносить только из MainItemCollection НПС.")]
    public SharedVariable<bool> OnlyFromMain = true;

    [Tooltip("Возвращать Success, даже если переносить нечего.")]
    public SharedVariable<bool> SucceedWhenEmpty = true;

    private Inventory _fromInv;
    private Inventory _warehouseInv;
    private ItemCollection _targetCollection;
    private ItemCategory _category;
    private ItemCollection _fromMain;

    public override void OnStart()
    {
        _fromInv = gameObject.GetComponentInParent<Inventory>();
        _warehouseInv = Warehouse?.Value != null ? Warehouse.Value.GetComponentInParent<Inventory>() : null;

        _targetCollection = _warehouseInv == null
            ? null
            : (string.IsNullOrEmpty(WarehouseCollectionName?.Value)
                ? _warehouseInv.MainItemCollection
                : _warehouseInv.GetItemCollection(WarehouseCollectionName.Value)); // UIS: GetItemCollection.

        _category = string.IsNullOrEmpty(CategoryName?.Value)
            ? null
            : InventorySystemManager.GetItemCategory(CategoryName.Value); // UIS: Inventory System Manager.

        _fromMain = _fromInv != null ? _fromInv.MainItemCollection : null;
    }

    public override TaskStatus OnUpdate()
    {
        if (_fromInv == null || _targetCollection == null || _category == null) return TaskStatus.Failure;

        var all = _fromInv.AllItemInfos; // UIS: все ItemInfo/стеки.
        var moved = false;

        for (int i = 0; i < all.Count; i++)
        {
            var ii = all[i];
            if (ii.Item == null) continue;
            if (OnlyFromMain.Value && ii.ItemCollection != _fromMain) continue;

            var def = ii.Item.ItemDefinition;
            var amount = ii.Amount;
            if (def == null || amount <= 0) continue;

            // Фильтр по категории (с учётом дочерних).
            if (!_category.InherentlyContains(def)) continue; // Проверка принадлежности к категории. 

            // Сколько склад реально примет под эту Definition?
            var canAdd = _targetCollection.CanAddItem(new ItemInfo(def,
                amount)); // возвращает ItemInfo? с частичным Amount.
            if (!canAdd.HasValue || canAdd.Value.Amount <= 0) continue;

            // Снять именно из ЭТОГО стека: сконструировать ItemInfo на базе ii с нужным количеством.
            var toRemove = new ItemInfo(canAdd.Value.Amount, ii);
            var removed = ii.ItemCollection.RemoveItem(toRemove); // фактически снятое кол-во (не nullable). 
            if (removed.Amount <= 0) continue;

            // Положить на склад.
            _targetCollection.AddItem(new ItemInfo(def, removed.Amount)); // добавление в целевую коллекцию. 
            moved = true;
        }

        return (moved || SucceedWhenEmpty.Value) ? TaskStatus.Success : TaskStatus.Failure;
    }
}
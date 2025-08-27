/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController.Editor
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.Shared.Editor.UIElements.Controls;
    using Opsive.Shared.Editor.UIElements.Controls.Types;
    using Opsive.UltimateCharacterController.Character.Abilities;
    using Opsive.UltimateCharacterController.Character.Abilities.Items;
    using Opsive.UltimateCharacterController.Character.Effects;
    using Opsive.UltimateCharacterController.Editor.Controls.Attributes;
    using Opsive.UltimateCharacterController.Inventory;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;

    /// <summary>
    /// Draws a custom inspector for the Behavior Designer - Ultimate Character Controller ItemSet categories.
    /// </summary>
    [ControlType(typeof(CategoryID))]
    public class ItemSetCategoryControl: TypeControlBase
    {
        /// <summary>
        /// Does the control use a label?
        /// </summary>
        public override bool UseLabel { get { return false; } }

        /// <summary>
        /// Returns the control that should be used for the specified ControlType.
        /// </summary>
        /// <param name="input">The input to the control.</param>
        /// <returns>The created control.</returns>
        protected override VisualElement GetControl(TypeControlInput input)
        {
            // ItemSetManagerBase must exist for the categories to be populated.
            var itemSetManager = GameObject.FindObjectOfType<ItemSetManagerBase>();
            var container = new VisualElement();
            var dropdownContainer = new VisualElement();
            if (itemSetManager == null) {
                var objectField = new ObjectField();
                objectField.objectType = typeof(ItemSetManagerBase);
                objectField.RegisterValueChangedCallback(c =>
                {
                    itemSetManager = c.newValue as ItemSetManagerBase;
                    dropdownContainer.Clear();
                    AddItemSetManagerCategories(input, dropdownContainer, itemSetManager);
                });
                container.Add(new LabelControl("Item Set Manager", input.Tooltip, objectField));
            }

            container.Add(dropdownContainer);
            AddItemSetManagerCategories(input, dropdownContainer, itemSetManager);

            return container;
        }

        /// <summary>
        /// Adds the categories from the specified ItemSetManager.
        /// </summary>
        /// <param name="input">The input to the control.</param>
        /// <param name="container">The parent container.</param>
        /// <param name="itemSetManager">The ItemSetManager that the categories should be retrieved from.</param>
        private void AddItemSetManagerCategories(TypeControlInput input, VisualElement container, ItemSetManagerBase itemSetManager)
        {
            // Draw a popup with all of the ItemSet groups.
            var categoryID = ((CategoryID)input.Value).ID;
            var categoryNames = new List<string>() { "(none)" };
            var index = GetIndex(itemSetManager, categoryNames, categoryID);
            var hasItemSetGroups = itemSetManager != null && itemSetManager.ItemSetGroups != null && itemSetManager.ItemSetGroups.Length > 0;
            if (index == -1) {
                if (hasItemSetGroups) {
                    index = 1;
                    input.OnChangeEvent(new CategoryID() { ID = itemSetManager.ItemSetGroups[0].CategoryID });
                } else {
                    if (categoryID != 0) {
                        index = 1;
                        categoryNames.Add(categoryID.ToString());
                    } else {
                        index = 0;
                    }
                }
            }

            var dropdownField = new DropdownField(categoryNames, index);
            dropdownField.SetValueWithoutNotify(categoryNames[index]);
            var labelControl = new LabelControl(input.Label, input.Tooltip, dropdownField, LabelControl.WidthAdjustment.UnityDefault);
            System.Action<object> onBindingUpdateEvent = (object newValue) => {
                var index = GetIndex(itemSetManager, null, ((CategoryID)newValue).ID);
                if (index == -1) {
                    index = 0;
                }
                dropdownField.SetValueWithoutNotify(categoryNames[index]);
            };
            dropdownField.RegisterCallback<AttachToPanelEvent>(c =>
            {
                BindingUpdater.AddBinding(input.Field, -1, input.Target, onBindingUpdateEvent);
            });
            dropdownField.RegisterCallback<DetachFromPanelEvent>(c =>
            {
                BindingUpdater.RemoveBinding(onBindingUpdateEvent);
            });
            dropdownField.RegisterValueChangedCallback(c =>
            {
                dropdownField.SetValueWithoutNotify(c.newValue);
                c.StopPropagation();
                if (hasItemSetGroups) {
                    input.OnChangeEvent(new CategoryID() { ID = itemSetManager.ItemSetGroups[dropdownField.index - 1].CategoryID });
                } else {
                    input.OnChangeEvent(new CategoryID() { ID = 0 });
                }
            });
            container.Add(labelControl);
        }

        /// <summary>
        /// Returns the index of the category selected.
        /// </summary>
        /// <param name="itemSetManager">The ItemSetManager that the categories should be retrieved from.</param>
        /// <param name="categoryNames">The names of the categories.</param>
        /// <param name="categoryID">The ID of the category.</param>
        /// <returns></returns>
        private int GetIndex(ItemSetManagerBase itemSetManager, List<string> categoryNames, uint categoryID)
        {
            var index = -1;
            if (itemSetManager != null) {
                itemSetManager.Initialize(false);
                if (itemSetManager.ItemSetGroups != null) {
                    for (int i = 0; i < itemSetManager.ItemSetGroups.Length; ++i) {
                        if (categoryNames != null) {
                            categoryNames.Add(itemSetManager.ItemSetGroups[i].CategoryName);
                        }
                        if (categoryID == itemSetManager.ItemSetGroups[i].CategoryID) {
                            index = i + 1;
                        }
                    }
                }
            }
            return index;
        }
    }

    /// <summary>
    /// Draws a custom inspector for the Behavior Designer - Ultimate Character Controller ability and effects.
    /// </summary>
    [ControlType(typeof(AbilityString))]
    [ControlType(typeof(ItemSetAbilityString))]
    [ControlType(typeof(EffectString))]
    public class AbilityEffectStringControl : TypeControlBase
    {
        /// <summary>
        /// Does the control use a label?
        /// </summary>
        public override bool UseLabel { get { return false; } }

        /// <summary>
        /// Returns the control that should be used for the specified ControlType.
        /// </summary>
        /// <param name="input">The input to the control.</param>
        /// <returns>The created control.</returns>
        protected override VisualElement GetControl(TypeControlInput input)
        {
            System.Type dropdownType;
            var field = input.Value.GetType().GetField("Type", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            string typeValue;
            if (input.Value is AbilityString abilityString) {
                dropdownType = typeof(Ability);
                typeValue = abilityString.Type;
            } else if (input.Value is ItemSetAbilityString itemSetAbilityString) {
                dropdownType = typeof(ItemSetAbilityBase);
                typeValue = itemSetAbilityString.Type;
            } else { // EffectString.
                dropdownType = typeof(Effect);
                typeValue = ((EffectString)input.Value).Type;
            }

            return new DropdownSelectionAttributeControl.DropdownSelectionObjectStringView(dropdownType, field, input.Value, typeValue, input.Label, input.Tooltip,
                (object obj) =>
                {
                    if (input.Value is AbilityString) {
                        return input.OnChangeEvent(new AbilityString() { Type = obj as string });
                    } else if (input.Value is ItemSetAbilityString) {
                        return input.OnChangeEvent(new ItemSetAbilityString() { Type = obj as string });
                    } else { // EffectString.
                        return input.OnChangeEvent(new EffectString() { Type = obj as string });
                    }
                });
        }
    }
}
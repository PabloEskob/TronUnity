/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateInventorySystem
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.UltimateInventorySystem.Core;
    using UnityEngine;

    [NodeDescription("Open, Close or Toggle a panel using its unique name.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class OpenClosePanel : Action
    {
        [Tooltip("The Id for the Display Panel Manager.")]
        public SharedVariable<int> m_DisplayPanelManagerID;
        [Tooltip("The unique name of the Panel.")]
        public SharedVariable<string> m_PanelUniqueName;
        [Tooltip("Toggle the panel on and off?")]
        public SharedVariable<bool> m_Toggle;
        [Tooltip("Close or Open the panel?")]
        public SharedVariable<bool> m_Close;
        [Tooltip("Close the selected panel if none with the name are found?")]
        public SharedVariable<bool> m_CloseSelected;

        /// <summary>
        /// Returns success if the inventory has the item amount specified, otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            var displayManager = InventorySystemManager.GetDisplayPanelManager(System.Convert.ToUInt32(m_DisplayPanelManagerID.Value));
            if (displayManager == null) {
                Debug.LogWarning("The Display Panel Manager was not found.");
                return TaskStatus.Failure;
            }
            
            if (m_Toggle.Value) {
                displayManager.TogglePanel(m_PanelUniqueName.Value);
            } else {
                if (m_Close.Value) {
                    var panel = displayManager.GetPanel(m_PanelUniqueName.Value);

                    if (panel != null) {
                        panel.Close();
                    } else if(m_CloseSelected.Value) {
                        displayManager.CloseSelectedPanel();
                    }
                    
                } else {
                    displayManager.OpenPanel(m_PanelUniqueName.Value);
                }
            }

            return TaskStatus.Success;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_DisplayPanelManagerID = 0;
            m_PanelUniqueName = string.Empty;
            m_Toggle = false;
            m_Close = false;
            m_CloseSelected = false;
        }
    }
}
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using System.Collections.Generic;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace Opsive.BehaviorDesigner.Integrations.UltimateCharacterController
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.Shared.StateSystem;
    using UnityEngine;

    [NodeDescription("Sets the set on the target GameObject.")]
    [NodeIcon("b52e2c467cd28924cb6c3d19ffcb822a")]
    public class SetNewState : TargetGameObjectAction
    {
        [System.Serializable]
        public class StateToggle
        {
            [Tooltip("State name (case sensitive).")]
            public SharedVariable<string> StateName;

            [Tooltip("Activate (true) or Deactivate (false).")]
            public SharedVariable<bool> Active = true;
        }

        [Tooltip("States to set in order (each entry is Name + On/Off).")]
        public StateToggle[] m_States;

        [Header("Speed Change handling (optional)")] [Tooltip("If true, will stop the Speed Change ability before applying states.")]
        public bool m_StopSpeedChange = false;

        private Ability _speedChange;
        private UltimateCharacterLocomotion _UltimateCharacterLocomotion;

        public override void OnAwake()
        {
            base.OnAwake();

            _UltimateCharacterLocomotion = gameObject.GetComponent<UltimateCharacterLocomotion>();
            if (_UltimateCharacterLocomotion != null)
            {
                _speedChange = _UltimateCharacterLocomotion.GetAbility<SpeedChange>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (m_States == null || m_States.Length == 0)
            {
                return TaskStatus.Success;
            }

            if (m_StopSpeedChange)
            {
                _UltimateCharacterLocomotion.TryStopAbility(_speedChange);
            }

            if (m_States != null)
            {
                for (int i = 0; i < m_States.Length; i++)
                {
                    var e = m_States[i];
                    if (e == null || e.StateName == null) continue;
                    var name = e.StateName.Value;
                    if (string.IsNullOrEmpty(name)) continue;
                    var on = e.Active != null ? e.Active.Value : true;
                    StateManager.SetState(gameObject, name, on);
                }
            }

            return TaskStatus.Success;
        }

        public override void Reset()
        {
            m_States = null;
        }
    }
}
/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Integrations.Cinemachine
{
    using UnityEngine;
    using Unity.Cinemachine;

    /// <summary>
    /// Updates the Cinemachine Brain in the correct order with the SimulationManager.
    /// </summary>
    public class CinemachineUpdater : MonoBehaviour
    {
        CinemachineBrain m_Brain;

        private void Awake()
        {
            m_Brain = GetComponent<CinemachineBrain>();

            m_Brain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;
            m_Brain.BlendUpdateMethod = CinemachineBrain.BrainUpdateMethods.LateUpdate;
        }

        private void LateUpdate()
        { 
            m_Brain.ManualUpdate();
        }
    }
}
/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Integrations.Cinemachine
{
    /// <summary>
    /// Implements a Third Person ViewType for the Cinemachine ViewType.
    /// </summary>
    [Shared.StateSystem.AddState("Aim", "275b4b43359185041be5c13c85a158d7")]
    public class ThirdPersonCinemachine : CinemachineViewType
    {
        public override bool FirstPersonPerspective { get { return false; } }
    }
}

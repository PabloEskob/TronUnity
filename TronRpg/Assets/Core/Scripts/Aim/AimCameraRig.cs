using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.Aim
{
    [ExecuteAlways]
    public class AimCameraRig : CinemachineCameraManagerBase, IInputAxisOwner
    {
        public InputAxis AimMode = InputAxis.DefaultMomentary;

        private SimplePlayerAimController _aimController;
        private CinemachineVirtualCameraBase _aimCamera;
        private CinemachineVirtualCameraBase _freeCamera;

        bool IsAiming => AimMode.Value > 0.5f;

        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref AimMode, Name = "Aim" });
        }

        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < ChildCameras.Count; ++i)
            {
                var cam = ChildCameras[i];
                if (!cam.isActiveAndEnabled)
                    continue;
                if (_aimCamera == null
                    && cam.TryGetComponent<CinemachineThirdPersonAim>(out var aim)
                    && aim.NoiseCancellation)
                {
                    _aimCamera = cam;
                    var player = _aimCamera.Follow;
                    if (player != null)
                        _aimController = player.GetComponentInChildren<SimplePlayerAimController>();
                }
                else if (_freeCamera == null)
                    _freeCamera = cam;
            }

            if (_aimCamera == null)
                Debug.LogError("AimCameraRig: no valid CinemachineThirdPersonAim camera found among children");
            if (_aimController == null)
                Debug.LogError("AimCameraRig: no valid SimplePlayerAimController target found");
            if (_freeCamera == null)
                Debug.LogError("AimCameraRig: no valid non-aiming camera found among children");
        }

        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            var oldCam = (CinemachineVirtualCameraBase)LiveChild;
            var newCam = IsAiming ? _aimCamera : _freeCamera;
            
            if (_aimController && oldCam != newCam)
            {
                _aimController.PlayerRotation = IsAiming
                    ? SimplePlayerAimController.CouplingMode.Coupled
                    : SimplePlayerAimController.CouplingMode.Decoupled;
                _aimController.RecenterPlayer();
            }
            return newCam;
        }
    }
}
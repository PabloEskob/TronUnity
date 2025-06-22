using UnityEngine;
using Config.Camera;

namespace Core.Camera.States
{
    public abstract class CameraStateBase : ICameraState
    {
        protected readonly CameraStateManager _stateManager;
        protected readonly CameraController _controller;
        protected readonly Transform _transform;

        protected Transform Target => _stateManager.Target;
        protected CameraConfig Config => _controller.ActiveConfig;

        protected CameraStateBase(CameraStateManager stateManager, CameraController controller)
        {
            _stateManager = stateManager;
            _controller = controller;
            _transform = controller.transform;
        }

        public virtual void EnterState()
        {
        }

        public abstract void UpdateState();
        public abstract void LateUpdateState();

        public virtual void ExitState()
        {
        }
    }
}
namespace Core.Camera.States
{
    public interface ICameraState
    {
        void EnterState();
        void UpdateState();
        void LateUpdateState();
        void ExitState();
    }
}
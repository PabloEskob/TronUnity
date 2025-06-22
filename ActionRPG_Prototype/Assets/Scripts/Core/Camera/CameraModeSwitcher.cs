using UnityEngine;
using Unity.Cinemachine;

public enum CameraMode
{
    Free,
    Combat,
    LockOn
}

public class CameraModeSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase freeLook;
    [SerializeField] private CinemachineVirtualCameraBase combat;
    [SerializeField] private CinemachineVirtualCameraBase lockOn;
    [SerializeField] private CinemachineTargetGroup targetGroup;

    private const int FREE_PRI = 10;
    private const int COMBAT_PRI = 11;
    private const int LOCK_PRI = 12;

    public void SetMode(CameraMode mode)
    {
        freeLook.Priority = mode == CameraMode.Free ? FREE_PRI : 0;
        combat.Priority = mode == CameraMode.Combat ? COMBAT_PRI : 0;
        lockOn.Priority = mode == CameraMode.LockOn ? LOCK_PRI : 0;
    }

    public void SetLockOnTarget(Transform enemy)
    {
        if (enemy == null) return;
        // element 0 = player (создайте заранее)
        targetGroup.Targets[1].Object = enemy;
        SetMode(CameraMode.LockOn);
    }
}
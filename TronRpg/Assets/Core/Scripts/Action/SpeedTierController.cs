using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Pathfinding;
using UnityEngine;

public class SpeedTierController : Action
{
    [Header("Пороги (м)")] public SharedVariable<float> WalkMax = 2.0f; // ≤ 2 -> Walk
    public SharedVariable<float> JogMax = 5.0f; // (2..5] -> Jog, >5 -> Run

    [Header("Гистерезис (м)")] public SharedVariable<float> Hyst = 0.3f;

    [Header("State names (на компоненте Speed Change)")]
    public SharedVariable<string> JogState = "Speed_Jog";

    public SharedVariable<string> RunState = "Speed_Run";
    
    [Header("Политика при отсутствии пути/цели")]
    public SharedVariable<bool> NoPathIsWalk = true; // если нет пути -> Walk
    public SharedVariable<bool> NoDestIsWalk = true; // если destination не задан -> Walk
    public SharedVariable<float> MaxClamp = 1000f;   // верхний зажим расстояния (safety)

    [Header("Опции")] public SharedVariable<bool> StopSpeedChangeOnWalk = true; // при Walk гасим способность и все стейты

    private IAstarAI _ai;
    private UltimateCharacterLocomotion _ucl;
    private SpeedChange _speed;
    private Tier _current = Tier.Unknown;

    private enum Tier
    {
        Unknown,
        Walk,
        Jog,
        Run
    }

    public override void OnAwake()
    {
        _ai = GetComponent<IAstarAI>();
        _ucl = GetComponent<UltimateCharacterLocomotion>();
        _speed = _ucl != null ? _ucl.GetAbility<SpeedChange>() : null;
    }

    public override TaskStatus OnUpdate()
    {
        if (_ai == null || _ucl == null || _speed == null) return TaskStatus.Running;

        float d = ComputeDistanceSafely();
        var desired = DecideTier(d);

        if (desired != _current) {
            ApplyTier(desired);
            _current = desired;
        }

        return TaskStatus.Running;
    }
    
    private float ComputeDistanceSafely()
    {
        var dest = _ai.destination;
        bool destInvalid = float.IsNaN(dest.x) || float.IsInfinity(dest.x);

        if (_ai.reachedDestination) return 0f; // A*: «достигнут» по endReachedDistance. :contentReference[oaicite:3]{index=3}

        if (_ai.hasPath && !_ai.pathPending) {
            var rd = _ai.remainingDistance; // может быть +∞, если пути нет. :contentReference[oaicite:4]{index=4}
            if (!float.IsNaN(rd) && !float.IsInfinity(rd))
                return Mathf.Clamp(rd, 0f, MaxClamp.Value);
        }

        if (!destInvalid) {
            var p = _ucl.transform.position;
            var fallback = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(dest.x, dest.z));
            return Mathf.Clamp(fallback, 0f, MaxClamp.Value);
        }

        return (NoPathIsWalk.Value || NoDestIsWalk.Value) ? 0f : MaxClamp.Value;
    }

    private Tier DecideTier(float d)
    {
        // Гистерезис: пороги расширяем/сужаем в зависимости от текущего состояния.
        var walkMaxUp = WalkMax.Value + Hyst.Value;
        var walkMaxDn = WalkMax.Value - Hyst.Value;
        var jogMaxUp = JogMax.Value + Hyst.Value;
        var jogMaxDn = JogMax.Value - Hyst.Value;

        switch (_current)
        {
            case Tier.Walk:
                if (d > jogMaxUp) return Tier.Run;
                if (d > walkMaxUp) return Tier.Jog;
                return Tier.Walk;

            case Tier.Jog:
                if (d <= walkMaxDn) return Tier.Walk;
                if (d > jogMaxUp) return Tier.Run;
                return Tier.Jog;

            case Tier.Run:
                if (d <= walkMaxDn) return Tier.Walk;
                if (d <= jogMaxDn) return Tier.Jog;
                return Tier.Run;

            default:
                if (d <= WalkMax.Value) return Tier.Walk;
                if (d <= JogMax.Value) return Tier.Jog;
                return Tier.Run;
        }
    }

    private void ApplyTier(Tier t)
    {
        // Радио-группа стейтов: сначала всё погасить.
        if (!string.IsNullOrEmpty(JogState.Value)) StateManager.SetState(gameObject, JogState.Value, false);
        if (!string.IsNullOrEmpty(RunState.Value)) StateManager.SetState(gameObject, RunState.Value, false);
        // См. StateManager.SetState в доках UCC. :contentReference[oaicite:5]{index=5}

        if (t == Tier.Walk) {
            if (StopSpeedChangeOnWalk.Value && _speed.IsActive) _ucl.TryStopAbility(_speed);
            return; // остаётся Default скорость.
        }

        // Для Jog/Run — просто запускаем способность (без «гейта по движению»).
        if (!_speed.IsActive) {
            _ucl.TryStartAbility(_speed); // См. UCC Speed Change. :contentReference[oaicite:6]{index=6}
        }

        if (t == Tier.Jog && !string.IsNullOrEmpty(JogState.Value))
            StateManager.SetState(gameObject, JogState.Value, true);

        if (t == Tier.Run && !string.IsNullOrEmpty(RunState.Value))
            StateManager.SetState(gameObject, RunState.Value, true);
    }
}
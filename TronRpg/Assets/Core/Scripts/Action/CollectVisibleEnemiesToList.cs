using System.Collections.Generic;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

/// <summary>
/// Собирает видимых врагов в радиусе в SharedGameObjectList с учётом FOV и Line of Sight.
/// Спроектирован по SRP: узел только "перцепция". Выбор цели/атака — отдельные узлы.
/// </summary>
public class CollectVisibleEnemiesToList : Action
{
    // === Выход ===
    [Tooltip("Сюда будет записан массив обнаруженных врагов (для Tactical Pack).")]
    public SharedVariable<GameObject[]> Targets; // <-- массив, совместим с TacticalBase.m_Targets

    // === Геометрия обзора ===
    public SharedVariable<float> MaxDistance = 25f;
    public SharedVariable<Vector2> FieldOfViewAngle = new Vector2(120f, 90f); // (гориз., вертик.), градусы
    public SharedVariable<Vector3> PivotOffset = new Vector3(0f, 1.6f, 0f);
    public SharedVariable<Vector3> TargetOffset = new Vector3(0f, 0.5f, 0f);

    // === Фильтры и физика ===
    [Tooltip("Слои врагов для OverlapSphereNonAlloc.")]
    public LayerMask EnemyLayers = ~0;

    [Tooltip("Слои, которые игнорировать при Linecast (свои коллайдеры и т.п.).")]
    public LayerMask IgnoreRaycastMask = 0;

    public SharedVariable<bool> IncludeTriggersInOverlap = true;
    public SharedVariable<bool> RequireLineOfSight = true;

    // === Ограничители нагрузки ===
    public SharedVariable<int> MaxCandidates = 64; // буфер Overlap
    public SharedVariable<float> UpdateInterval = 0.25f; // сек, частота сканирования
    public SharedVariable<int> MaxLOSPerScan = 16; // квота на Linecast
    public SharedVariable<bool> SortByDistance = true; // сортировать по расстоянию

    // --- Runtime ---
    private Collider[] _overlap;
    private readonly List<GameObject> _results = new List<GameObject>(64);
    private Transform _tr;
    private float _nextScanTime;
    private TaskStatus _cached = TaskStatus.Failure;
    private readonly HashSet<int> _seenIds = new HashSet<int>(128);

    public override void OnAwake()
    {
        _tr = transform;
        ResizeBuffer();
    }

    public override void OnStart() => _nextScanTime = 0f;

    public override TaskStatus OnUpdate()
    {
        if (Time.time < _nextScanTime) return _cached;
        _nextScanTime = Time.time + Mathf.Max(0.01f, UpdateInterval.Value);
        ResizeBuffer();

        _results.Clear();
        _seenIds.Clear();

        // 1) Кандидаты
        var origin = _tr.TransformPoint(PivotOffset.Value);
        var qti = IncludeTriggersInOverlap.Value ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

        int count = Physics.OverlapSphereNonAlloc(
            _tr.position,
            Mathf.Max(0.01f, MaxDistance.Value),
            _overlap,
            EnemyLayers,
            qti
        );

        // Подготовка FOV
        Vector3 fwd = _tr.forward;
        Vector3 fwdHoriz = new Vector3(fwd.x, 0f, fwd.z);
        if (fwdHoriz.sqrMagnitude < 1e-6f) fwdHoriz = Vector3.forward;
        fwdHoriz.Normalize();

        float halfHoriz = Mathf.Max(0f, FieldOfViewAngle.Value.x * 0.5f);
        float halfVert = Mathf.Max(0f, FieldOfViewAngle.Value.y * 0.5f);
        float maxDist = Mathf.Max(0.01f, MaxDistance.Value);
        float maxDistS = maxDist * maxDist;

        int losUsed = 0;

        for (int i = 0; i < count; i++)
        {
            var col = _overlap[i];
            if (!col) continue;

            // Берём "владельца" коллайдера: Rigidbody-root, если есть, иначе сам объект коллайдера.
            var go = col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.gameObject;
            if (!go) continue;

            // >>> Дедупликация: один и тот же владелец может иметь несколько коллайдеров.
            int id = go.GetInstanceID();
            if (!_seenIds.Add(id)) continue; // уже добавлен ранее — пропускаем

            Vector3 targetPos = go.transform.TransformPoint(TargetOffset.Value);
            Vector3 dir = targetPos - origin;

            // Дистанция
            if (dir.sqrMagnitude > maxDistS) continue;

            // Горизонтальный угол
            Vector3 horizDir = new Vector3(dir.x, 0f, dir.z);
            if (horizDir.sqrMagnitude < 1e-6f) continue;
            horizDir.Normalize();
            if (Vector3.Angle(horizDir, fwdHoriz) > halfHoriz) continue;

            // Вертикальный угол
            float vertAngle = Vector3.Angle(dir, new Vector3(dir.x, 0f, dir.z));
            if (vertAngle > halfVert) continue;

            // LOS (опционально, с квотой)
            if (RequireLineOfSight.Value && losUsed++ < Mathf.Max(0, MaxLOSPerScan.Value))
            {
                if (Physics.Linecast(origin, targetPos, out var hit, ~IgnoreRaycastMask, QueryTriggerInteraction.Ignore))
                {
                    // Пропускаем, если луч упёрся не в цель/её детей.
                    if (!hit.transform.IsChildOf(go.transform) && !go.transform.IsChildOf(hit.transform)) continue;
                }
            }

            _results.Add(go);
        }

        // Сортировка по расстоянию (опционально)
        if (SortByDistance.Value && _results.Count > 1)
        {
            var p = _tr.position;
            _results.Sort((a, b) =>
                ((a.transform.position - p).sqrMagnitude).CompareTo((b.transform.position - p).sqrMagnitude));
        }

        // 2) Переводим в массив и обновляем SharedVariable<GameObject[]>
        int newLen = _results.Count;
        var newArr = new GameObject[newLen];
        for (int i = 0; i < newLen; i++) newArr[i] = _results[i];

        // Не дергаем OnValueChange, если состав не поменялся
        var oldArr = Targets.Value;
        bool changed = oldArr == null || oldArr.Length != newArr.Length;
        if (!changed && oldArr != null)
        {
            for (int i = 0; i < oldArr.Length; i++)
            {
                if (oldArr[i] != newArr[i])
                {
                    changed = true;
                    break;
                }
            }
        }

        if (changed || oldArr == null)
            Targets.Value = newArr;

        _cached = newLen > 0 ? TaskStatus.Success : TaskStatus.Failure;
        return _cached;
    }

    public override void Reset()
    {
        MaxDistance = 25f;
        FieldOfViewAngle = new Vector2(120f, 90f);
        PivotOffset = new Vector3(0f, 1.6f, 0f);
        TargetOffset = new Vector3(0f, 0.5f, 0f);
        EnemyLayers = ~0;
        IgnoreRaycastMask = 0;
        IncludeTriggersInOverlap = true;
        RequireLineOfSight = true;
        MaxCandidates = 64;
        UpdateInterval = 0.25f;
        MaxLOSPerScan = 16;
        SortByDistance = true;
        Targets = null;
    }

    private void ResizeBuffer()
    {
        int cap = Mathf.Max(8, MaxCandidates.Value);
        if (_overlap == null || _overlap.Length != cap)
            _overlap = new Collider[cap];
    }

    protected override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // NB: у задач BD Pro есть transform (GameObject Task).
        var t = transform;
        if (t == null) return;

        // Радиус
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawWireSphere(t.position, Mathf.Max(0.01f, MaxDistance.Value));

        // FOV (горизонтальный сектор)
        float halfHoriz = Mathf.Max(0f, FieldOfViewAngle.Value.x * 0.5f);
        var left = Quaternion.AngleAxis(-halfHoriz, Vector3.up) * t.forward;
        var right = Quaternion.AngleAxis(halfHoriz, Vector3.up) * t.forward;

        Gizmos.color = new Color(1f, 1f, 1f, 0.35f);
        Gizmos.DrawLine(t.position, t.position + left * MaxDistance.Value);
        Gizmos.DrawLine(t.position, t.position + right * MaxDistance.Value);

        // Точка зрения
        var origin = t.TransformPoint(PivotOffset.Value);
        Gizmos.color = new Color(0.8f, 0.8f, 1f, 0.6f);
        Gizmos.DrawSphere(origin, 0.05f);
#endif
    }
}
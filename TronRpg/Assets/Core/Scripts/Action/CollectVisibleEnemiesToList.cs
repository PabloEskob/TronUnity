using System;
using System.Collections.Generic;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

namespace Core.Scripts.Action
{
    /// <summary>
    /// Собирает видимых врагов в радиусе в SharedGameObjectList с учётом FOV и Line of Sight.
    /// Спроектирован по SRP: узел только "перцепция". Выбор цели/атака — отдельные узлы.
    /// </summary>
    public class CollectVisibleEnemiesToList : Opsive.BehaviorDesigner.Runtime.Tasks.Actions.Action
    {
        private struct DetectionCandidate
        {
            public GameObject Target;
            public Vector3 TargetPosition;
            public float SquaredDistance;
        }

        [SerializeField] protected SharedVariable<Vector2> FieldOfViewAngle; // x=гориз., y=вертик. (в градусах)
        [SerializeField] protected SharedVariable<Vector3> PivotOffset; // смещение «глаз» сенсора
        [SerializeField] protected SharedVariable<Vector3> TargetOffset; // точка на цели (например, центр/голова)
        [SerializeField] protected SharedVariable<float> UpdateInterval;
        [SerializeField] protected SharedVariable<float> MaxDistance;
        [SerializeField] protected SharedVariable<int> MaxLOSPerScan;
        [SerializeField] protected SharedVariable<bool> RequireLineOfSight;
        [SerializeField] protected SharedVariable<bool> SortByDistance;
        [SerializeField] protected SharedVariable<bool> IncludeTriggersInOverlap;
    
        [SerializeField] protected LayerMask EnemyLayers;
        [SerializeField] protected LayerMask IgnoreRaycastMask;
    
        [SerializeField] protected SharedVariable<GameObject[]> Targets;
    
        private Transform _characterTransform;
        private float _nextScanTime;

        private readonly List<DetectionCandidate> _candidates = new List<DetectionCandidate>(64);
        private readonly List<GameObject> _results = new List<GameObject>(64);
        private readonly HashSet<int> _seenInstanceIds = new HashSet<int>();
        private Collider[] _overlapBuffer;

        private TaskStatus _cached = TaskStatus.Failure;

        public override void OnAwake()
        {
            _characterTransform = transform;
            EnsureBufferCapacity(128);
        }

        public override TaskStatus OnUpdate()
        {
            if (!ShouldScan()) return _cached;

            ScheduleNextScan();
            EnsureBufferCapacity(_overlapBuffer?.Length ?? 128);
            ClearWorkingSets();

            Vector3 origin = GetOrigin();
            float maxDistance = Mathf.Max(0.01f, MaxDistance.Value);
            int overlapCount = OverlapCandidates(origin, maxDistance, GetQueryTriggerInteraction());

            FovParams fov = PrepareFovParams();
            float maxDistanceSquared = maxDistance * maxDistance;
            int losBudget = Mathf.Max(0, MaxLOSPerScan.Value);

            BuildCandidates(origin, overlapCount, in fov, maxDistanceSquared, ref losBudget);

            SortCandidatesIfRequested();

            _results.Clear();
            for (int i = 0; i < _candidates.Count; i++)
                _results.Add(_candidates[i].Target);

            AssignTargetsArrayIfChanged(_results);

            _cached = _results.Count > 0 ? TaskStatus.Success : TaskStatus.Failure;
            return _cached;
        }


        private bool ShouldScan() => Time.time >= _nextScanTime;

        private void ScheduleNextScan()
        {
            _nextScanTime = Time.time + Mathf.Max(0.01f, UpdateInterval.Value);
        }

        private void EnsureBufferCapacity(int desiredCapacity)
        {
            if (_overlapBuffer == null || _overlapBuffer.Length < desiredCapacity)
                _overlapBuffer = new Collider[Mathf.Max(desiredCapacity, 64)];
        }

        private void ClearWorkingSets()
        {
            _candidates.Clear();
            _seenInstanceIds.Clear();
            _results.Clear();
        }

        private Vector3 GetOrigin() => _characterTransform.TransformPoint(PivotOffset.Value);

        private QueryTriggerInteraction GetQueryTriggerInteraction() =>
            IncludeTriggersInOverlap.Value ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

        private int OverlapCandidates(Vector3 origin, float maxDistance, QueryTriggerInteraction queryTrigger)
        {
            // NonAlloc — без GC
            return Physics.OverlapSphereNonAlloc(origin, maxDistance, _overlapBuffer, EnemyLayers, queryTrigger);
        }

        private static GameObject GetOwnerGameObject(Collider collider)
        {
            if (!collider) return null;
            return collider.attachedRigidbody ? collider.attachedRigidbody.gameObject : collider.gameObject;
        }

        private readonly struct FovParams
        {
            public readonly Vector3 ForwardHorizontal;
            public readonly float CosineHalfHorizontal;
            public readonly float SineHalfVertical;

            public FovParams(Vector3 forwardHorizontal, float cosineHalfHorizontal, float sineHalfVertical)
            {
                ForwardHorizontal = forwardHorizontal;
                CosineHalfHorizontal = cosineHalfHorizontal;
                SineHalfVertical = sineHalfVertical;
            }
        }

        private FovParams PrepareFovParams()
        {
            Vector3 forward = _characterTransform.forward;
            Vector3 forwardHorizontal = new Vector3(forward.x, 0f, forward.z);
            if (forwardHorizontal.sqrMagnitude < 1e-6f) forwardHorizontal = Vector3.forward;
            forwardHorizontal.Normalize();

            float halfHorizontalDegrees = Mathf.Max(0f, FieldOfViewAngle.Value.x * 0.5f);
            float halfVerticalDegrees = Mathf.Max(0f, FieldOfViewAngle.Value.y * 0.5f);

            float cosineHalfHorizontal = Mathf.Cos(halfHorizontalDegrees * Mathf.Deg2Rad);
            float sineHalfVertical = Mathf.Sin(halfVerticalDegrees * Mathf.Deg2Rad);

            return new FovParams(forwardHorizontal, cosineHalfHorizontal, sineHalfVertical);
        }

        private void BuildCandidates(
            Vector3 origin,
            int overlapCount,
            in FovParams fov,
            float maxDistanceSquared,
            ref int losBudget)
        {
            for (int i = 0; i < overlapCount; i++)
            {
                Collider collider = _overlapBuffer[i];
                if (!collider) continue;

                GameObject owner = GetOwnerGameObject(collider);
                if (!owner) continue;

                int instanceId = owner.GetInstanceID();
                if (!_seenInstanceIds.Add(instanceId)) continue; // один владелец с несколькими коллайдерами

                Vector3 targetPosition = owner.transform.TransformPoint(TargetOffset.Value);
                Vector3 direction = targetPosition - origin;

                // --- Дистанция ---
                float squaredDistance = direction.sqrMagnitude;
                if (squaredDistance > maxDistanceSquared) continue;

                float horizontalLengthSquared = direction.x * direction.x + direction.z * direction.z;
                if (horizontalLengthSquared < 1e-6f) continue;

                float inverseHorizontalLength = 1f / Mathf.Sqrt(horizontalLengthSquared);
                float dotHorizontal =
                    (direction.x * inverseHorizontalLength) * fov.ForwardHorizontal.x +
                    (direction.z * inverseHorizontalLength) * fov.ForwardHorizontal.z;

                if (dotHorizontal < fov.CosineHalfHorizontal) continue;

                if (squaredDistance < 1e-12f) continue; // защита от деления на 0
                float inverseLength = 1f / Mathf.Sqrt(squaredDistance);
                float sineElevation = Mathf.Abs(direction.y) * inverseLength;
                if (sineElevation > fov.SineHalfVertical) continue;

                // --- LOS с бюджетом ---
                if (RequireLineOfSight.Value)
                {
                    if (losBudget <= 0) continue; // или break — если нужен жёсткий потолок на тик
                    losBudget--;

                    if (Physics.Linecast(origin, targetPosition, out RaycastHit hitInfo, ~IgnoreRaycastMask, QueryTriggerInteraction.Ignore))
                    {
                        // Пропускаем, если луч упёрся не в цель/её детей
                        if (!hitInfo.transform.IsChildOf(owner.transform) && !owner.transform.IsChildOf(hitInfo.transform))
                            continue;
                    }
                }

                _candidates.Add(new DetectionCandidate
                {
                    Target = owner,
                    TargetPosition = targetPosition,
                    SquaredDistance = squaredDistance
                });
            }
        }

        private void SortCandidatesIfRequested()
        {
            if (!SortByDistance.Value || _candidates.Count <= 1) return;
            _candidates.Sort((left, right) => left.SquaredDistance.CompareTo(right.SquaredDistance));
        }

        private void AssignTargetsArrayIfChanged(List<GameObject> source)
        {
            int count = source.Count;
            GameObject[] current = Targets.Value;

            bool needsAssign = current == null || current.Length != count;
            if (!needsAssign)
            {
                for (int i = 0; i < count; i++)
                {
                    if (current[i] != source[i])
                    {
                        needsAssign = true;
                        break;
                    }
                }
            }

            if (!needsAssign) return;

            if (count == 0)
            {
                Targets.Value = Array.Empty<GameObject>();
                return;
            }

            var newArray = new GameObject[count];
            source.CopyTo(newArray);
            Targets.Value = newArray;
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
}
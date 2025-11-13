using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Pathfinding;
using UnityEngine;

namespace Core.Scripts.Action
{
    public class RotateTowardsAI : Opsive.BehaviorDesigner.Runtime.Tasks.Actions.Action
    {
        [SerializeField] private SharedVariable<bool> m_UpdateRotation = true;
        [SerializeField] private bool m_Use2DPhysics = false;
        [SerializeField] private SharedVariable<float> m_MaxRotationSpeedDegPerSec = 360f; // вместо "на тик"
        [SerializeField] private SharedVariable<float> m_ArrivedAngle = 0.5f;
        [SerializeField] private SharedVariable<bool> m_OnlyY = true;
        [SerializeField] private SharedVariable<GameObject> m_Target;
        [SerializeField] private SharedVariable<Vector3> m_TargetRotation;
    
        private IAstarAI _ai;
        private bool m_StartUpdateRotation;

        public override void OnAwake()
        {
            _ai = gameObject.GetComponent<IAstarAI>();
            if (_ai == null) {
                Debug.LogError($"Error: Unable to find the IAstarAI component on the {gameObject} GameObject.");
                return;
            }
        }

        public override void OnStart()
        {
            m_StartUpdateRotation = _ai.updateRotation;
            UpdateRotation(m_UpdateRotation.Value);
        }

        public override TaskStatus OnUpdate()
        {
            var targetRot = GetTargetRotation();
            var current = transform.rotation;

            if (Quaternion.Angle(current, targetRot) <= m_ArrivedAngle.Value)
                return TaskStatus.Success;

            // Переведём "скорость в град/сек" в "угол за кадр".
            var step = Mathf.Max(0f, m_MaxRotationSpeedDegPerSec.Value) * (Time.deltaTime > 0 ? Time.deltaTime : 0.02f);
            var next = Quaternion.RotateTowards(current, targetRot, step);

            // Пишем именно в transform.rotation, т.к. updateRotation у A* отключён.
            transform.rotation = next;

            // (Опционально) синхронизируем "внутренний" rotation A* для совместимости.
            if ( _ai != null) _ai.rotation = next;

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UpdateRotation(m_StartUpdateRotation);
        }

        private Quaternion GetTargetRotation()
        {
            if (m_Target.Value == null) return Quaternion.Euler(m_TargetRotation.Value);

            var dir = m_Target.Value.transform.position - transform.position;

            if (m_OnlyY.Value) dir.y = 0f;

            if (dir.sqrMagnitude < 1e-6f) return transform.rotation; // защита на месте

            if (m_Use2DPhysics)
            {
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                return Quaternion.AngleAxis(angle, Vector3.forward);
            }

            return Quaternion.LookRotation(dir.normalized);
        }
    
        private void UpdateRotation(bool update)
        {
            _ai.updateRotation = update;
        }

        public override void Reset()
        {
            m_Use2DPhysics = false;
            m_MaxRotationSpeedDegPerSec = 360f;
            m_ArrivedAngle = 0.5f;
            m_OnlyY = true;
            m_Target = null;
            m_TargetRotation = Vector3.zero;
        }
    }
}
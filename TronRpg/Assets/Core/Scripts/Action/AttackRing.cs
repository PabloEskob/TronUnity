using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

namespace Opsive.BehaviorDesigner.AddOns.TacticalPack.Runtime.Tasks
{
    [Opsive.Shared.Utility.Description("Атака с окружением цели. Агентам выдаются равные точки по кольцу.")]
    [DefaultAttackDelay(AttackDelay.Arrival)]
    public class AttackRing : TacticalBase
    {
        [Tooltip("Фиксированный радиус кольца (м). 0 = авто от MinAttackDistance.")]
        public SharedVariable<float> RingRadius = 0f;

        [Tooltip("Минимальный зазор между волками по дуге (м).")]
        public SharedVariable<float> ArcSpacing = 0.9f;

        [Tooltip("Смещение центра кольца вперёд по направлению группы (м).")]
        public SharedVariable<float> ForwardBias = 0f;

        public override Vector3 CalculateFormationPosition(int index, int totalAgents,
            Vector3 center, Vector3 forward, bool samplePosition)
        {
            if (m_AttackTarget != null) center = m_AttackTarget.position;

            var up = m_Is2D ? Vector3.forward : Vector3.up;
            if (forward.sqrMagnitude < 1e-4f) forward = transform.forward;
            forward = Vector3.ProjectOnPlane(forward, up).normalized;

            // Авторадиус: не ближе минимальной дистанции атаки и с учётом зазора по окружности.
            float radius = RingRadius.Value > 0 ? RingRadius.Value
                : Mathf.Max(m_AttackAgent.MinAttackDistance, 0.75f);
            if (totalAgents > 1 && ArcSpacing.Value > 0f) {
                var needCircumference = totalAgents * ArcSpacing.Value;
                radius = Mathf.Max(radius, needCircumference / (2f * Mathf.PI));
            }

            var angleStep = 360f / Mathf.Max(1, totalAgents);
            var angle = index * angleStep;
            var dir = Quaternion.AngleAxis(angle, up) * forward;
            var pos = center + dir * radius + forward * ForwardBias.Value;

            if (samplePosition && !SamplePosition(ref pos)) {
                // Небольшой даунскейл радиуса, если точка вне графа.
                for (int i = 0; i < 6 && !SamplePosition(ref pos); i++) {
                    radius *= 0.85f;
                    pos = center + dir * radius;
                }
            }
            return pos;
        }
    }
}

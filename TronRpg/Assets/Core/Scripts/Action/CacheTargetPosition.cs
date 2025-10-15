using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

public class CacheTargetPosition : Action
{
    [Tooltip("Целевой трансформ")]
    public SharedVariable<GameObject> Target;

    [Tooltip("Сюда запишем позицию")]
    public SharedVariable<Vector3> OutPosition;

    public override TaskStatus OnUpdate()
    {
        var t = Target?.Value;
        if (t == null) return TaskStatus.Failure;
        OutPosition.Value = t.transform.position;
        return TaskStatus.Success;
    }
}
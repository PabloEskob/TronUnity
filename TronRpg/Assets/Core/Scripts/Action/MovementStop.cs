using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Pathfinding;

public class MovementStop : Action
{
    private IAstarAI _ai;
    public override void OnAwake() => _ai = GetComponent<IAstarAI>();

    public override TaskStatus OnUpdate()
    {
        if (_ai == null) return TaskStatus.Failure;
        _ai.isStopped = true;
        _ai.SetPath(null);
        _ai.canSearch = false;
        return TaskStatus.Success;
    }
}
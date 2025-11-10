using Opsive.BehaviorDesigner.AddOns.MovementPack.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime;
using Opsive.Shared.Events;
using UnityEngine;

public class PatrolAI : Patrol
{
    private BehaviorTree _behaviorTree;

    public override void OnAwake()
    {
        base.OnAwake();
        _behaviorTree = GetComponent<BehaviorTree>();
    }

    public override void OnStart()
    {
        base.OnStart();
        
        OnWaypointArrival -= HandleWaypointArrival;
        OnWaypointArrival += HandleWaypointArrival;
    }

    public override void OnEnd()
    {
        OnWaypointArrival -= HandleWaypointArrival;
        base.OnEnd();
    }

    public override void OnDestroy()
    {
        OnWaypointArrival -= HandleWaypointArrival;
        base.OnDestroy();
    }
    
    private void HandleWaypointArrival(GameObject waypoint)
    {
        if (_behaviorTree != null) 
            EventHandler.ExecuteEvent<object>(_behaviorTree, "OnWaypointArrival", waypoint);
    }
}
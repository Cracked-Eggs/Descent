using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : NodeBase
{
    public List<Transform> fleePoints;
    public MonsterMovementController controller;
    public MonsterVariables variables;

    public override void OnTransition()
    {
        controller.Run();
        controller.Move(variables.spider.position);
    }

    public override void OnExit()
    {
        controller.Walk();
    }

    public override void Tick()
    {
        if (!controller.ReachedTarget())
            return;

        int furthestTargetIndex = 0;
        float furthestTargetDistance = 0.0f;

        for (int i = 0; i < fleePoints.Count; i++)
        {
            float distanceToWaypoint = (variables.spider.position - fleePoints[i].position).magnitude;

            if (distanceToWaypoint >= furthestTargetDistance)
            {
                furthestTargetIndex = i;
                furthestTargetDistance = distanceToWaypoint;
            }
        }

        controller.Move(fleePoints[furthestTargetIndex].position);
    }
}

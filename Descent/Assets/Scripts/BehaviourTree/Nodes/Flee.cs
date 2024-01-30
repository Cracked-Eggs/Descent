using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : NodeBase
{
    public List<Transform> fleePoints;
    public MonsterMovementController controller;
    public MonsterVariables variables;
    public float lowerDotProductBounds;
    public float upperDotProductBounds;
    public float minPlayerDistanceFromWaypoint;

    public EventObject deManifest;
    public EventObject manifest;

    public override void Tick()
    {
        int bestWaypointIndex = -1;

        for (int i = 0; i < fleePoints.Count; i++)
        {
            Vector3 toWaypoint = variables.spider.position - fleePoints[i].position;
            Vector3 waypointToPlayer = variables.player.position - fleePoints[i].position;

            float dotProduct = Mathf.Abs(Vector3.Dot(toWaypoint.normalized, variables.playerCamera.forward));

            RaycastHit hit;
            if (waypointToPlayer.magnitude < minPlayerDistanceFromWaypoint)
            {
                continue;
            }

            if (dotProduct > lowerDotProductBounds && dotProduct < upperDotProductBounds)
            {
                bestWaypointIndex = i;
                break;
            }
        }

        if (bestWaypointIndex == -1)
        {
            manifest.Invoke(true);
            deManifest.Invoke(false);
            Debug.Log("Could not find waypoint, killing instead");
            return;
        }

        controller.Run();
        controller.Move(fleePoints[bestWaypointIndex].position);
    }
}

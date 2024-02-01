using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manifest : NodeBase
{
    public MonsterMovementController controller;
    public Transform monsterEmpty;
    public GameObject monsterBody;
    public Transform runner;
    public MonsterVariables variables;
    public List<Transform> spawnPoints;

    public override void OnTransition()
    {
        monsterBody.SetActive(false);

        float furthestDistance = 0.0f;
        int spawnPointIndex = 0;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Transform currentSpawnPoint = spawnPoints[i];
            float distanceToPlayer = (variables.player.position - currentSpawnPoint.position).magnitude;
            if (distanceToPlayer > furthestDistance)
            {
                furthestDistance = distanceToPlayer;
                spawnPointIndex = i;
            }
        }

        monsterEmpty.position = spawnPoints[spawnPointIndex].position;
        monsterBody.transform.position = spawnPoints[spawnPointIndex].position;
        runner.transform.parent = null;
        runner.position = spawnPoints[spawnPointIndex].position;
        runner.transform.parent = monsterEmpty.transform;

        monsterBody.SetActive (true);
        controller.Manifest();
        controller.Walk();
    }
}

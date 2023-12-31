using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    public MonsterVariables variables;
    public float playerFOV;
    public float playerVisionRange;
    public EventObject playerCantSeeEvent;

    void Update()
    {
        Vector3 toPlayer = (variables.player.position - variables.spider.position);
        if (toPlayer.magnitude >= playerVisionRange)
        {
            playerCantSeeEvent.Invoke(true);
            return;
        }

        RaycastHit hit;
        if(!Physics.Raycast(variables.spider.position, toPlayer.normalized, out hit))
        {
            playerCantSeeEvent.Invoke(true);
            return;
        }

        if (hit.collider.name != variables.playerName)
        {
            playerCantSeeEvent.Invoke(true);
            return;
        }

        if(Vector3.Dot(variables.player.forward, -toPlayer.normalized) < playerFOV)
        {
            playerCantSeeEvent.Invoke(true);
            return;
        }

        playerCantSeeEvent.Invoke(false);
    }
}

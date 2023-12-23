using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRange : MonoBehaviour
{
    public EventObject rangeEvent;
    public Transform player;
    public Transform monster;
    public float detectionRange;

    void Update()
    {
        if((player.position - monster.position).magnitude < detectionRange)
        {
            rangeEvent.Invoke(true);
        }
        else
        {
            rangeEvent.Invoke(false);
        }
    }
}

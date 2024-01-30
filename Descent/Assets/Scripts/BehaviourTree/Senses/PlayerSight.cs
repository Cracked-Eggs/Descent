using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    public MonsterVariables variables;
    public float playerFOV;
    public float playerVisionRange;
    public EventObject playerCantSeeEvent;
    public SkinnedMeshRenderer spiderRenderer;
    public LayerMask wallLayer;
    public float missThreshold;

    void FixedUpdate()
    {
        Vector3 toPlayer = (variables.player.position - variables.spiderEyes.position);

        RaycastHit hit;
        if(!Physics.Raycast(variables.spiderEyes.position, toPlayer.normalized, out hit, Mathf.Infinity, wallLayer))
        {
            playerCantSeeEvent.Invoke(true);
            return;
        }

        if (hit.collider.name != variables.playerName && (hit.point - variables.player.position).magnitude > missThreshold)
        {
            playerCantSeeEvent.Invoke(true);
            return;
        }

        if (spiderRenderer.isVisible)
        {
            playerCantSeeEvent.Invoke(false);
        }
        else
        {
            playerCantSeeEvent.Invoke(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaulting : MonoBehaviour
{
    private int vaultLayer;
    public Camera cam;
    private float playerHeight = 2f;
    private float playerRadius = 0.5f;

    void Start()
    {
        vaultLayer = LayerMask.NameToLayer("VaultLayer");
        vaultLayer = ~vaultLayer;
    }

    public void PerformAutoVault()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var firstHit, 1f, vaultLayer))
        {
            Debug.Log("vaultable in front");
            if (Physics.Raycast(firstHit.point + (cam.transform.forward * playerRadius) + (Vector3.up * 0.6f * playerHeight), Vector3.down, out var secondHit, playerHeight))
            {
                Debug.Log("found place to land");
                StartCoroutine(LerpVault(secondHit.point, 0.5f));
            }
        }
    }

    IEnumerator LerpVault(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}

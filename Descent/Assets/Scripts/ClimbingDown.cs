using System.Collections;
using UnityEngine;

public class ClimbingDown : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    public Transform playerTransform;
    PlayerMovement pm;

    [Header("Climbing Down")]
    [SerializeField] float maxDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] float downwardSpeed;
    [SerializeField] float distanceIncrement;

    bool isClimbingDown;

    void Awake() => pm = GetComponent<PlayerMovement>();

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1)) StartClimbingDown();
    }

    void StartClimbingDown()
    {
        if (isClimbingDown) return;

        isClimbingDown = true;
        pm.freeze = true;

        StartCoroutine(ClimbDown());
    }

    IEnumerator ClimbDown()
    {
        for (float currentDistance = 0f; currentDistance < maxDistance; currentDistance += distanceIncrement)
        {
            Vector3 rayOrigin = cam.position + cam.forward * currentDistance;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
            {
                // Store the origin position and the hit position.
                Vector3 originPosition = rayOrigin;
                Vector3 hitPosition = hit.point;

                // Move the player to the origin position first.
                while (Vector3.Distance(playerTransform.position, originPosition) > 0.1f)
                {
                    playerTransform.position = Vector3.MoveTowards(playerTransform.position, originPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                // Check if downward rays from the player hit wall layers.
                if (!IsHittingWallLayers())
                {
                    // Start moving the player to the hit position.
                    while (Vector3.Distance(playerTransform.position, hitPosition) > 0.1f)
                    {
                        playerTransform.position = Vector3.MoveTowards(playerTransform.position, hitPosition, downwardSpeed * Time.deltaTime);
                        yield return null;
                    }
                }

                isClimbingDown = false;
                pm.freeze = false;
                yield break;
            }

            yield return null;
        }

        // Stop climbing down if no ground is found or the maximum distance is reached.
        isClimbingDown = false;
        pm.freeze = false;
    }

    bool IsHittingWallLayers()
    {
        Vector3 playerPosition = playerTransform.position;
        return Physics.Raycast(playerPosition, Vector3.down, out _, 1f, wallLayer);
    }
}

    using SA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PlayerController playerMovement;
    public FreeClimb climbingScript;
    public LedgeChecker checker;

    private CharacterController characterController; // Change from Rigidbody to CharacterController
    private Quaternion initialRotation;

    private GameObject climbHelperObject;

    private void Start()
    {
        playerMovement = GetComponent<PlayerController>();
        climbingScript = GetComponent<FreeClimb>();
        characterController = GetComponent<CharacterController>(); // Change from Rigidbody to CharacterController

        initialRotation = transform.rotation; // Use transform.rotation instead of rb.rotation

        SetWalkingState();
    }

    public void SetWalkingState()
    {
        playerMovement.enabled = true;
        playerMovement.freeze = false;
        climbingScript.enabled = false;
        climbingScript.isClimbing = false;

        transform.rotation = initialRotation; // Use transform.rotation instead of rb.rotation
        characterController.detectCollisions = true; // Enable collisions for CharacterController
        isClimbing = false;

        if (climbHelperObject != null)
        {
            Destroy(climbHelperObject);
        }
    }
    private bool CheckForValidWall()
    {
        Vector3 origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;

        // Perform a raycast to check for a wall in front of the player
        if (Physics.Raycast(origin, dir, out hit, 5))
        {
            // If a wall is detected, it's considered a valid wall
            return true;
        }

        // If no wall is detected, it's not a valid wall
        return false;
    }

    public void SetClimbingState()
    {
        playerMovement.enabled = false;
        playerMovement.freeze = true;
        climbingScript.enabled = true;

        // Check for a valid wall before creating the climb helper GameObject
        if (CheckForValidWall())
        {
            // Create the climb helper GameObject if it doesn't exist.
            if (climbHelperObject == null)
            {
                climbHelperObject = new GameObject("climb helper");
            }

            climbingScript.helper = climbHelperObject.transform; // Assign the helper Transform to the climbing script's helper field.
            climbingScript.CheckForClimb();
            
            isClimbing = true;
            // Reset the position of the climb helper GameObject
            climbHelperObject.transform.position = transform.position;
        }
        else
        {
            Debug.Log("No valid wall to climb. Cannot switch to climbing state.");
            SetWalkingState(); // Reset the state to walking
            isClimbing = false;
            
        }
    }

    private bool isClimbing = false;
    private Vector3 jumpVelocity;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isClimbing)
        {
            SetClimbingState();
        }

        if (checker.isLedgeDetected == true)
        {
            ClimbLedge();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SetWalkingState();
        }
    }

    public void ClimbLedge()
    {
        if (climbingScript.isClimbing && checker.isLedgeDetected)
        {
            if (CanClimbLedge())
            {
                Vector3 climbOffset = Vector3.up * 5f;
                Vector3 newPosition = climbingScript.helper.position + climbOffset;

                // Use CharacterController.Move to set the new position
                characterController.Move(newPosition - transform.position);

                SetWalkingState();
            }
        }
    }

    private bool CanClimbLedge()
    {
        float distanceToLedge = Vector3.Distance(transform.position, climbingScript.helper.position);
        if (distanceToLedge < 1.0f)
        {
            return true;
        }

        return false;
    }

}

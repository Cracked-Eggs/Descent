using SA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public FreeClimb climbingScript;
    public LedgeChecker checker;

    private Rigidbody rb;
    private Quaternion initialRotation; // Store the initial rotation of the player mesh

    private GameObject climbHelperObject; // Store a reference to the climb helper GameObject.


    private void Start()
    {
        // Get references to the PlayerMovement and FreeClimb scripts
        playerMovement = GetComponent<PlayerMovement>();
        climbingScript = GetComponent<FreeClimb>();
        rb = GetComponent<Rigidbody>();

        // Store the initial rotation
        initialRotation = rb.rotation;


        // Initialize the player in the walking state
        SetWalkingState();
    }

    public void SetWalkingState()
    {
        playerMovement.enabled = true;
        playerMovement.freeze = false;
        climbingScript.enabled = false;
        climbingScript.isClimbing = false;

        // Reset the Rigidbody's rotation to the initial rotation
        rb.rotation = initialRotation;
        rb.useGravity = false;
        isClimbing = false ;

        // Destroy the climb helper GameObject if it exists.
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
            rb.useGravity = false;
            isClimbing = true;
            // Reset the position of the climb helper GameObject
            climbHelperObject.transform.position = transform.position;
        }
        else
        {
            Debug.Log("No valid wall to climb. Cannot switch to climbing state.");
            SetWalkingState(); // Reset the state to walking
            isClimbing = false;
            rb.useGravity = true;
        }
    }

    private bool isClimbing = false;
    private Vector3 jumpVelocity;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isClimbing)
        {
            SetClimbingState();
         ;
        }

        if (checker.isLedgeDetected == true)
        {
            ClimbLedge(); // Check for climbing over the ledge
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
                Vector3 climbOffset = Vector3.up * 5f; // Adjust the height as needed
                Vector3 newPosition = climbingScript.helper.position + climbOffset;
                rb.MovePosition(newPosition);

                SetWalkingState();
                rb.useGravity = true;
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

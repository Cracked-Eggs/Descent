    using SA;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PlayerController playerMovement;
    public Vaulting vaultingScript;
    public FreeClimb climbingScript;
    public LedgeChecker checker;

    private CharacterController characterController; 
    private Quaternion initialRotation;

    private GameObject climbHelperObject;

    private bool validWall;

    public GameObject climbableWall;
    private void Start()
    {
        init();

    }

    void init()
    {
        SetWalkingState();
        vaultingScript = GetComponent<Vaulting>();
        playerMovement = GetComponent<PlayerController>();
        climbingScript = GetComponent<FreeClimb>();
        characterController = GetComponent<CharacterController>();

        initialRotation = transform.rotation; // Use transform.rotation instead of rb.rotation
    }

    public void SetWalkingState()
    {
        playerMovement.enabled = true;
        playerMovement.freeze = false;
        climbingScript.enabled = false;
        climbingScript.isClimbing = false;
        isClimbing = false;

        transform.rotation = initialRotation; // Use transform.rotation instead of rb.rotation
        characterController.detectCollisions = true; // Enable collisions for CharacterController
        isClimbing = false;
        Destroy(climbHelperObject);

        if (climbHelperObject != null)
        {
            Destroy(climbHelperObject);
        }
    }
    public bool CheckForValidWall()
    {
        Vector3 origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;

        // Perform a raycast to check for a wall in front of the player
        if (Physics.Raycast(origin, dir, out hit, 5))
        {
            // Check if the hit object has the "climbable wall" tag
            if (hit.collider.gameObject == climbableWall)
            {
                validWall = true;

                return true;
            }
        }

        // If no wall is detected or the detected wall doesn't have the specified tag, it's not a valid wall
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
        
        CheckForValidWall();
        if (checker.isLedgeDetected == true)
        {
            vaultingScript.PerformAutoVault();
            SetWalkingState();
            isClimbing = false;// Call the vaulting method when a ledge is detected
            
        }

        if (Input.GetKeyDown(KeyCode.Z) && !isClimbing && CheckForValidWall() == true)
        {
            SetClimbingState();
        }

        if (Input.GetKeyDown(KeyCode.X) && isClimbing)
        {
            SetWalkingState();
            
            validWall = false;
        }
    }
}

 using SA;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField, Range(0, 360)] float upperlookLimit = 270f;
    [SerializeField, Range(0, 360)] float lowerlookLimit = 90f;
    public PlayerController playerMovement;
    public Vaulting vaultingScript;
    public FreeClimb climbingScript;
    public LedgeChecker checker;
    Animator anim;

    private CharacterController characterController; 
    private Quaternion initialRotation;
    private Quaternion currRotation;
    private Quaternion prevRotation;

    private GameObject climbHelperObject;
    public GameObject picks;
    private bool isClimbing = false;
    private Vector3 jumpVelocity;

    private bool validWall;

    public GameObject climbableWall;

    private Quaternion previousRotation;
    private Vector3 previousEulerAngles;

    public MouseLook ml;


    private void Start()
    {
        init();
        previousRotation = transform.rotation;
        previousEulerAngles = transform.eulerAngles;
    }

    void init()
    {
        
        SetWalkingState();
        vaultingScript = GetComponent<Vaulting>();
        playerMovement = GetComponent<PlayerController>();
        climbingScript = GetComponent<FreeClimb>();
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        initialRotation = transform.rotation; // Use transform.rotation instead of rb.rotation
    }

    public void SetWalkingState()
    {
        playerMovement.enabled = true;
        playerMovement.freeze = false;
        climbingScript.enabled = false;
        climbingScript.isClimbing = false;
        isClimbing = false;
        isClimbing = false;
        isClimbing = false;

        //transform.rotation = initialRotation; // Use transform.rotation instead of rb.rotation
        //characterController.detectCollisions = true; // Enable collisions for CharacterController
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
        if (Physics.Raycast(origin, dir, out hit, 1 ))
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

    private void Update()
    {
        Vector3 currentEulerAngles = transform.eulerAngles;
       

        if (isClimbing)
        {

           
            currentEulerAngles.y = Mathf.Clamp(currentEulerAngles.y, climbingScript.helper.eulerAngles.y - 45f, climbingScript.helper.eulerAngles.y + 45f);

            transform.eulerAngles = currentEulerAngles;

        }
        

        
        CheckForValidWall();
        if (checker.isLedgeDetected == true)
        {
            vaultingScript.PerformAutoVault();
            SetWalkingState();
            anim.SetTrigger("ExitClimbTrigger");
            isClimbing = false;// Call the vaulting method when a ledge is detected
            
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isClimbing && CheckForValidWall() == true)
        {
            if(picks.activeSelf)
            {
                SetClimbingState();
            }
           else
            {
                Debug.Log("Equip picks");
            }
        }

        if (!isClimbing && climbingScript.enabled == true)
        {
            isClimbing = true;
        }

        if (Input.GetKeyDown(KeyCode.X) && isClimbing)
        {
            SetWalkingState();
            
            
        }
       
    }
}

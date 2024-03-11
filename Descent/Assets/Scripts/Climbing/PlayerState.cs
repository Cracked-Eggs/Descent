 using SA;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField, Range(0, 360)] float upperlookLimit = 270f;
    [SerializeField, Range(0, 360)] float lowerlookLimit = 90f;
    public _CharacterController playerMovement;
    public Vaulting vaultingScript;
    public FreeClimb climbingScript;
    public LedgeChecker checker;
    Animator anim;

    InputActions actions;

    private CharacterController characterController; 
    private Quaternion initialRotation;
    private Quaternion currRotation;
    private Quaternion prevRotation;

    private GameObject climbHelperObject;
    public GameObject picks;
    public bool isClimbing = false;
    private Vector3 jumpVelocity;

    private bool validWall;

    public GameObject climbableWall;

    private Quaternion previousRotation;
    private Vector3 previousEulerAngles;

   

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
        playerMovement = GetComponent<_CharacterController>();
        climbingScript = GetComponent<FreeClimb>();
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        initialRotation = transform.rotation; // Use transform.rotation instead of rb.rotation
    }

    private void Awake()
    {
        actions =  new InputActions();
        actions.Enable();

        actions.Default.Jump.performed += e => Climbing();
    }

    public void SetWalkingState()
    {
        climbingScript.enabled = false;
        climbingScript.isClimbing = false;
        isClimbing = false;
  
        //transform.rotation = initialRotation; // Use transform.rotation instead of rb.rotation
        //characterController.detectCollisions = true; // Enable collisions for CharacterController

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
        if (checker.isLedgeDetected == true && isClimbing == true)
        {
            vaultingScript.PerformAutoVault();
            SetWalkingState();
            anim.SetTrigger("ExitClimbTrigger");
            isClimbing = false;// Call the vaulting method when a ledge is detected
            
        }


       


    }

    void Climbing()
    {
        if (isClimbing)
        {
            // If the player is already climbing, initiate the exit from climbing state
            SetWalkingState();
            isClimbing = false;
        }
        else if (CheckForValidWall())
        {
            // If the player is not climbing and there is a valid wall, check for picks and initiate climbing
            if (picks.activeSelf)
            {
                SetClimbingState();
            }
            else
            {
                Debug.Log("Equip picks");
            }
        }

    }
}

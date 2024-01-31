using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEditor.Rendering;
using static playerPrefss;
using System.Dynamic;
using Unity.VisualScripting;
using JetBrains.Annotations;
using SA;
using System.Runtime.CompilerServices;

public class _CharacterController : MonoBehaviour
{
    private CharacterController controller;
    private InputActions actions;
    CinemachineVirtualCamera virtualCamera;
    FreeClimb cb;
    PlayerStateManager sm;
    public LayerMask playerMask;

    private Vector2 inputMovement;
    private Vector2 inputView;
    Vector3 movementSpeed;

    private Vector3 CameraRot;
    private Vector3 CharacterRot;

    public Transform cameraHolder;
    public Transform feetTransform;

    
    public float defaultFOV = 60f; 
    public float sprintingFOV = 70f;

    public PlayerSettings playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;

    public float gravity = -9.8f;
    public float gravityMultiplier = 3f;
    public float velocity;
    public float jumpForce = 3.4f;
    public bool isJumping;

    public PlayerStance playerStance;
    public float playerStanceSmoothing;
 
    public CharacterStance playerStand;
    public CharacterStance playerCrouch;

    private float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;
    private float stanceCheckH = 1.5f;

   
    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;
    public bool test;

    private bool isSprinting;

    
    private Vector3 newVelocity;
    private Vector3 newMovementSpeed;

    bool canMove = true;

    public GroundCheck gc;

    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaConsumptionRate = 20f; 
    public float staminaRegenerationRate = 10f;

    private bool canSprint = true;

    void Awake()
    {
        actions = new InputActions();
        actions.Enable();

        actions.Default.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        actions.Default.Look.performed += e => inputView = e.ReadValue<Vector2>();

        actions.Default.Jump.performed += e => Jump();

        actions.Default.Crouch.performed += e => Crouch();
        

        actions.Default.Run.performed += e => ToggleSprint();
        actions.Default.RunReleased.performed += e => StopSprint();

        CameraRot = cameraHolder.localRotation.eulerAngles;
        CharacterRot = transform.localRotation.eulerAngles;

        controller = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;
        sm = GetComponent<PlayerStateManager>();
       
        virtualCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();

        currentStamina = maxStamina;


    }

    void Update()
    {
        canMove = !sm.isClimbing;
        View();
        if (canMove)
        {
            if (isSprinting)
            {
                currentStamina -= staminaConsumptionRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

                if (currentStamina <= 0)
                {
                    isSprinting = false;
                }
            }
            else
            {
                // Regenerate stamina when not sprinting
                currentStamina += staminaRegenerationRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }

            View();
            Movement();
            ApplyGravity();
            CalculateStance();
        }
       
    }

    private void Start()
    {
        cb = GetComponent<FreeClimb>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

  

    void View()
    {
         CharacterRot.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -inputView.x : inputView.x) * Time.deltaTime;

         transform.rotation = Quaternion.Euler(CharacterRot);

         CameraRot.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted ? inputView.y : -inputView.y) * Time.deltaTime;
         CameraRot.x = Mathf.Clamp(CameraRot.x, viewClampYMin, viewClampYMax);

         cameraHolder.localRotation = Quaternion.Euler(CameraRot);

        if (canMove)
        {
            float targetFOV = isSprinting ? sprintingFOV : defaultFOV;
            var currentFOV = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * playerSettings.FOVSensitivity);
            virtualCamera.m_Lens.FieldOfView = currentFOV;
        }
       

    }

    void Jump()
    {
        if (gc.isGrounded == false)
        {
            Debug.Log("Can't jump");
        }
        if (gc.isGrounded == true && playerStance == PlayerStance.Stand)
        {
            velocity = 0;
            isJumping = true;
            velocity += jumpForce;
            Debug.Log("jumping");
        }
         
        if (playerStance == PlayerStance.Crouch || playerStance == PlayerStance.Prone)
        {
          if (CheckHeadCollision(PlayerStance.Stand))
            {
              Debug.Log("Head collision detected! Unable to stand up.");
              return; // Head collision detected, cannot stand up
            }
        }

        playerStance = PlayerStance.Stand;
        
        movementSpeed.y = velocity;
        controller.Move(movementSpeed * Time.deltaTime);
        isJumping = false;
   
    }

    void ApplyGravity()
    {
        if (!gc.isGrounded)
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
            //Debug.Log("velocity" + velocity);
        }

        if (gc.isGrounded && velocity < 0)
        {
            velocity = 0;
        }


        movementSpeed.y = velocity;
        controller.Move(movementSpeed * Time.deltaTime);


    }
    void CalculateStance()
    {
        var currentStance = playerStand;
        if(playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouch;
        }
        
        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight,ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x,cameraHeight,cameraHolder.localPosition.z);

        controller.height = Mathf.SmoothDamp(controller.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        controller.center = Vector3.SmoothDamp(controller.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    void Movement()
    {
        if(inputMovement.y <= 0.2f)
        {
            isSprinting = false;
        }

        var verticalSpeed = playerSettings.WalkingSpeedF;
        var horizontalSpeed = playerSettings.WalkingSpeedS;

        if(isSprinting)
        {
            verticalSpeed = playerSettings.RunningSpeedF;
            horizontalSpeed = playerSettings.RunningSpeedS;
        }


        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * inputMovement.x * Time.deltaTime, 0, verticalSpeed * inputMovement.y * Time.deltaTime), ref newVelocity, playerSettings.MovementSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);
       
        controller.Move(movementSpeed);
    }

    void Crouch()
    {
        if (!isSprinting && (!CheckHeadCollision(PlayerStance.Crouch) || playerStance == PlayerStance.Stand)&& canMove)
        {
            if (playerStance == PlayerStance.Crouch)
            {
                playerStance = PlayerStance.Stand;
                canSprint = true;
                return;
            }
            playerStance = PlayerStance.Crouch;
            canSprint = false;
        }
    }
   

    bool CheckHeadCollision(PlayerStance newStance)
    {
        float raycastDistance;

        if (newStance == PlayerStance.Stand)
        {
            // When standing, use a shorter raycast distance to prevent hitting the ceiling
            raycastDistance = playerStand.StanceCollider.height * 0.1f;
        }
        else
        {
            // When crouching or proning, use the full raycast distance
            raycastDistance = playerStand.StanceCollider.height * 0.1f + stanceCheckH;
        }

        // Raycast to check for obstacles above the player's head
        Vector3 raycastOrigin = feetTransform.position; // You might need to adjust the origin based on your player's setup

        // Draw the ray for debugging
        Debug.DrawRay(raycastOrigin, Vector3.up * raycastDistance, Color.red, 0.5f);

        if (Physics.Raycast(raycastOrigin, Vector3.up, out RaycastHit hit, raycastDistance, playerMask))
        {
            
            if (hit.distance < raycastDistance + stanceCheckErrorMargin)
            {
                
                Debug.Log("Head collision detected! Unable to change stance.");
                return true; // Head collision detected, cannot change stance
            }
        }

        return false; 
    }

    void ToggleSprint()
    {
        if (playerStance == PlayerStance.Stand && inputMovement.y > 0.2f && canSprint && currentStamina > 0)
        {
            isSprinting = !isSprinting;
        }
        else
        {
            isSprinting = false;
        }
    }

    void StopSprint()
    {
        if(playerSettings.RunningHold)
        {
            isSprinting = false;
        }
        
    }
}

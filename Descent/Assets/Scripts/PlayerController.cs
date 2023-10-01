using UnityEngine.Events;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public class PlayerMovement : MonoBehaviour
{
    bool curry;
    public bool isMove { get; private set; } = true;
    private bool isSprinting => Sprinting && Input.GetKey(sprintKey);
    private bool isJumping => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    [Header("Look Settings")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperlookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerlookLimit = 80.0f;
    [SerializeField] Transform cam;
    int _clickCount = 0;
    
    public Transform position;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float sprintSpeed = 6.0f;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float jumpForce = 4.0f;
    [SerializeField] private bool Sprinting = true;
    [SerializeField] private bool Jumping = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool canUseFootsteps = true;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintAcceleration = 2.0f;
    [SerializeField] private float sprintDeceleration = 0.3f;
    private float currentSpeed = 0.0f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeed = 1.0f; 
    [SerializeField] private float standingHeight = 2.0f; 
    [SerializeField] private float crouchingHeight = 1.0f; 
    private bool isCrouching = false;



    [Header("Headbob")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobValue = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobValue = 0.05f;
    private float defaultYpos = 0;
    private float timer;


    [Header("Footsteps")]
    [SerializeField] float baseStepSpeed = 0.5f;
    [SerializeField] float sprintStepMultiplier = 0.6f;
    [SerializeField] AudioSource stepSource = default;
    [SerializeField] AudioClip[] steps = default;
    [SerializeField] AudioClip jump = default;
    float footstepTimer = 0;
    float GetCurrentOffset => isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    [Header("Look Settings")]
    [SerializeField] private float defaultFOV = 60.0f;
    [SerializeField] private float sprintingFOV = 70.0f;
    [SerializeField] private float fovLerpSpeed = 5.0f;

    public  Camera playerCamera;
    private float targetFOV;
    private float currentFOV;

    private bool isJumpingCheck = false;


    CharacterController characterController;
    

    private Vector3 moveDir;
    private Vector2 currInput;


    float _mouseMovementX = 0;


    

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        defaultYpos = cam.transform.localPosition.y;
        playerCamera = cam.GetComponent<Camera>();
        currentFOV = defaultFOV;
        currentSpeed = moveSpeed;

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {


        if (isMove)
        {
            MouseLook();
            Move();
            HandleCrouch();

            bool canJump = !isCrouching && characterController.isGrounded;


            if (Jumping && canJump)
            {
                if (Input.GetKeyDown(jumpKey))
                {
                    Jump(); // Trigger the jump
                    isJumpingCheck = true; // Set the flag to true
                }
            }

            if (canUseHeadbob)
                HeadBob();

            if (canUseFootsteps)
                Footsteps();

            bool isSprintKeyPressed = Input.GetKey(sprintKey) && Input.GetAxisRaw("Vertical") > 0;
            bool isCrouchKeyPressed = Input.GetKey(crouchKey);

            if (isCrouchKeyPressed && !isSprintKeyPressed)
            {
                curry = true;
            }
            else if (!isCrouchKeyPressed)
            {
                curry = false;
            }


            if (isSprintKeyPressed && !curry) // Check if sprint key is pressed
            {
                currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, Time.deltaTime * sprintAcceleration);
                targetFOV = sprintingFOV;
                Sprinting = true;
            }
            else if (!isSprintKeyPressed)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * sprintDeceleration);
                targetFOV = defaultFOV;
                Sprinting = false;

            }
            float currentMoveSpeed = isCrouching ? crouchSpeed : currentSpeed;



            currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovLerpSpeed * Time.deltaTime);
            playerCamera.fieldOfView = currentFOV;
        }

    }
    private void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey) && !isCrouching && !isSprinting)
        {
            StartCrouch();
        }
        else if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            StopCrouch();
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;

        characterController.height = crouchingHeight;
        currentSpeed = crouchSpeed;

    }

    private void StopCrouch()
    {
        isCrouching = false;

        characterController.height = standingHeight;

        
    }


    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
    }
   
    private void MouseLook()
    {
        _mouseMovementX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        _mouseMovementX = Mathf.Clamp(_mouseMovementX, -upperlookLimit, lowerlookLimit);
        cam.transform.localRotation = Quaternion.Euler(_mouseMovementX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);

    }
    private void Jump()
    {
        if (characterController.isGrounded)
        {
            Vector3 jumpDirection = moveDir;
            jumpDirection.y = jumpForce;

           
            moveDir = jumpDirection;

            //stepSource.PlayOneShot(jump);
        }
    }
    private void HeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDir.x) > 0.1f || Mathf.Abs(moveDir.z) > 0.1f)
        {
            timer += Time.deltaTime * (isSprinting ? sprintBobSpeed : walkBobSpeed);
            cam.transform.localPosition = new Vector3(
                cam.transform.localPosition.x, defaultYpos + Mathf.Sin(timer) * (isSprinting ? sprintBobValue : walkBobValue), cam.transform.localPosition.z);
        }
    }

    private void Footsteps()
    {
        if (!characterController.isGrounded) return;
        if (currInput == Vector2.zero) return;
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(cam.transform.position, Vector3.down, out RaycastHit hit, 3))
                switch (hit.collider.tag)
                {
                    case "Grass":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                }
            footstepTimer = GetCurrentOffset;
        }
    }
    private void Move()
    {

        
            currInput = new Vector2((isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Vertical"), (isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDir.y;
      
            moveDir = (transform.TransformDirection(Vector3.forward) * currInput.x) + (transform.TransformDirection(Vector3.right) * currInput.y);
        moveDir.y = moveDirectionY;

        
            moveDir.y -= gravity * Time.deltaTime;
        characterController.Move(moveDir * Time.deltaTime);

    }
}
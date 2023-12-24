using Cinemachine;
using UnityEngine;
using SA;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] float maxFallHeight;
    private float YPosBeforeJump;
    private bool jumpedOffLedge;

    [Header("Look Settings")]
    [SerializeField, Range(0, 1)] float lookSpeedX = 2.0f;
    [SerializeField, Range(0, 1)] float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] float upperlookLimit = 80.0f;
    [SerializeField, Range(1, 180)] float lowerlookLimit = 80.0f;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform position;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float sprintSpeed = 6.0f;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float gravityMultiplier = 3f;
    [SerializeField] float velocity;
    [SerializeField] float jumpForce = 3.4f;
    [SerializeField] bool Sprinting = true;
    [SerializeField] bool Jumping = true;
    [SerializeField] bool canUseHeadbob = true;
    [SerializeField] bool canUseFootsteps = true;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.C;

    [Header("Sprint Settings")]
    [SerializeField] float sprintAcceleration = 2.0f;
    [SerializeField] float sprintDeceleration = 0.3f;
    float currentSpeed = 0.0f;

    [Header("Crouch Settings")]
    [SerializeField] float crouchSpeed = 1.0f;
    [SerializeField] float standingHeight = 2.0f;
    [SerializeField] float crouchingHeight = 1.0f;
    [SerializeField] float crouchingSpeedSwitching = 1.0f;
    [SerializeField] float crouchingSpeed = 2.0f;
    [SerializeField] float targetHeight;
    bool isCrouching = false;

    [Header("Headbob")]
    [SerializeField] float walkBobSpeed = 14f;
    [SerializeField] float walkBobAmplitude = 0.05f;

    [SerializeField] float sprintBobSpeed = 18f;
    [SerializeField] float sprintBobAmplitude = 0.08f;

    [SerializeField] float crouchBobSpeed = 10f;
    [SerializeField] float crouchBobAmplitude = 0.01f;

    private float currentBobSpeed;
    private float currentBobAmplitude;

    float defaultYpos = 0;
    float timer;
    bool isJumping;

    [Header("Footsteps")]
    [SerializeField] float baseStepSpeed = 0.5f;
    [SerializeField] float sprintStepMultiplier = 0.6f;
    [SerializeField] AudioSource stepSource = default;
    [SerializeField] AudioClip[] steps = default;
    float footstepTimer = 0;
    float GetCurrentOffset => isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    [Header("Look Settings")]
    [SerializeField] float defaultFOV = 60.0f;
    [SerializeField] float sprintingFOV = 70.0f;
    [SerializeField] float fovLerpSpeed = 5.0f;

    bool curry;
    public bool isMove { get; private set; } = true;
    bool isSprinting => Sprinting && Input.GetKey(sprintKey);

    float targetFOV, currentFOV;
    float _mouseMovementX = 0;

    CharacterController characterController;
    Vector3 moveDir;
    Vector2 currInput;
    float jumpInput;
    
    FreeClimb cb;
    PlayerStateManager ps;
    bool wasClimbing;
    public bool canJump;
    [SerializeField]Transform groundCheck;
    [SerializeField]LayerMask ground;


    public MovementState state;
    public enum MovementState
    {
        freeze,
        climbing
    }

    public bool ropeClimbing;
    public bool climbing;
    public float climbSpeed;
    public bool freeze;
    
    public bool _canRestart;

    void Awake()
    {
        groundCheck = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        defaultYpos = virtualCamera.transform.localPosition.y;
        currentFOV = defaultFOV;
        currentSpeed = moveSpeed;
        currentBobSpeed = walkBobSpeed;
        currentBobAmplitude = walkBobAmplitude;
        targetHeight = standingHeight;
        cb = GetComponent<FreeClimb>();
        
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cb = GetComponent<FreeClimb>();
    }

    void Update()
    {
       
        Checker();
        
        if (_canRestart)
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);

        wasClimbing = climbing;
        climbing = false;

       

        if (ropeClimbing)
            return;
        Move();
        
        ApplyGravity();
        Jump();
        HandleCrouch();

        if (canUseHeadbob)
            HeadBob();
        if (canUseFootsteps)
            Footsteps();

        bool isSprintKeyPressed = Input.GetKey(sprintKey) && Input.GetAxisRaw("Vertical") > 0;
        bool isCrouchKeyPressed = Input.GetKey(crouchKey);

        if (isCrouchKeyPressed && !isSprintKeyPressed)
             curry = true;
        else if (!isCrouchKeyPressed)
             curry = false;

        if (isSprintKeyPressed && !curry) // Check if sprint key is pressed
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, Time.deltaTime * sprintAcceleration);
            currentBobSpeed = sprintBobSpeed;
            targetFOV = sprintingFOV;
            currentBobAmplitude = sprintBobAmplitude;
            Sprinting = true;
        }
        else if (!isSprintKeyPressed && !curry)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * sprintDeceleration);
            currentBobSpeed = walkBobSpeed;
            targetFOV = defaultFOV;
            currentBobAmplitude = walkBobAmplitude;
            Sprinting = false;
        }
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovLerpSpeed * Time.deltaTime);
        

        characterController.height = Mathf.Lerp(characterController.height, targetHeight,
            Time.deltaTime * crouchingSpeed);

        virtualCamera.m_Lens.FieldOfView = currentFOV;

        if (characterController.isGrounded)
        {
            if (!jumpedOffLedge)
                return;

            jumpedOffLedge = false;

            if (YPosBeforeJump -
                characterController.transform.position.y <= maxFallHeight)
                return;

            Debug.Log("You're dead");
        }
        else if (!characterController.isGrounded)
        {
            if (jumpedOffLedge)
                return;

            jumpedOffLedge = true;
            YPosBeforeJump = characterController.transform.position.y;
        }
    }
    void HandleCrouch()
    {
        if (!isJumping)
        {
            if (Input.GetKeyDown(crouchKey) && !isCrouching && !isSprinting)
                StartCrouch();
            else if (Input.GetKeyUp(crouchKey) && isCrouching)
                StopCrouch();
        }
        
    }

    void StartCrouch()
    {
        isCrouching = true;
        targetHeight = Mathf.Lerp(standingHeight,crouchingHeight,crouchingSpeedSwitching);
        currentSpeed = crouchSpeed;
        currentBobSpeed = crouchBobSpeed;
        currentBobAmplitude = crouchBobAmplitude;
    }

    void StopCrouch()
    {
        isCrouching = false;
        targetHeight = Mathf.Lerp(crouchingHeight, standingHeight, crouchingSpeedSwitching);
        currentBobSpeed = walkBobSpeed;
        currentBobAmplitude = walkBobAmplitude;
        currentSpeed = moveSpeed;
    }

   
   

    void HeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDir.x) > 0.1f || Mathf.Abs(moveDir.z) > 0.1f)
        {
            timer += Time.deltaTime * currentBobSpeed;

            virtualCamera.transform.localPosition = new Vector3(
            virtualCamera.transform.localPosition.x,
            defaultYpos + Mathf.Sin(timer) * currentBobAmplitude,
            virtualCamera.transform.localPosition.z);
        }
    }
    void Checker()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("isgrounded is:" + characterController.isGrounded);
        }
            
    }
    void Footsteps()
    {
        if (!characterController) return;
        if (currInput == Vector2.zero) return;
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(virtualCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
                switch (hit.collider.tag)
                {
                    case "Ground":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                    case "Safe":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                    case "PPlates":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                    case "DynamiteInteract":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                    case "UV Num":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                    case "NoDeath":
                        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length - 1)]);
                        break;
                }
            footstepTimer = GetCurrentOffset;
        }
    }


    
    void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
            //Debug.Log("velocity" + velocity);
        }

        if (characterController.isGrounded && velocity < 0)
        {
            velocity = 0;
        }

        
       
        
        moveDir.y = velocity;
        characterController.Move(moveDir * Time.deltaTime);


    }

    void Jump()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            if (characterController.isGrounded)
            {
                velocity  = 0;
                isJumping = true;
                velocity += jumpForce;
                Debug.Log("jumping");
            }
        }
        moveDir.y = velocity;
        characterController.Move(moveDir * Time.deltaTime);
        isJumping = false;
    }
    void Move()
    {
        
        
        if (ropeClimbing) return;

        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            characterController.SimpleMove(Vector3.zero);
        }

        currInput = new Vector2((isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Vertical"), (isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Horizontal"));
        
        moveDir = (transform.TransformDirection(Vector3.forward) * currInput.x) + (transform.TransformDirection(Vector3.right) * currInput.y);
        
        
       
        characterController.Move(moveDir * Time.deltaTime);
        jumpInput = 0;
       

    }
}
using UnityEngine.Events;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;


public class PlayerMovement : MonoBehaviour
{
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
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintSpeed = 6.0f;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float jumpForce = 4.0f;
    [SerializeField] private bool Sprinting = true;
    [SerializeField] private bool Jumping = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool canUseFootsteps = true;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;


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


    CharacterController characterController;
    

    private Vector3 moveDir;
    private Vector2 currInput;

    float _mouseMovementX = 0;

    

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        defaultYpos = cam.transform.localPosition.y;
        
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


            if (Jumping)
                Jump();

            if (canUseHeadbob)
                HeadBob();

            if (canUseFootsteps)
                Footsteps();
        }

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
        if (isJumping)
        {
            moveDir.y = jumpForce;
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
        if (characterController.isGrounded)
            currInput = new Vector2((isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Vertical"), (isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDir.y;
        if (characterController.isGrounded)
            moveDir = (transform.TransformDirection(Vector3.forward) * currInput.x) + (transform.TransformDirection(Vector3.right) * currInput.y);
        moveDir.y = moveDirectionY;

        if (!characterController.isGrounded)
            moveDir.y -= gravity * Time.deltaTime;
        characterController.Move(moveDir * Time.deltaTime);
    }
}
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Init")]
    public CharacterController controller;
    public CinemachineVirtualCamera playerCamera;
    public Transform groundCheck;
    public Transform headCheck;
    public float headCheckRadius;
    public float groundCheckRadius;
    public LayerMask groundMask;

    [Header("Player Prefs")]
    public Transform player;
    public float walkSpeed;
    public float runSpeed;
    public float runFOV;
    public float regularFOV;
    public float fOVLerpSpeed;
    public float crouchSpeed;
    public float standHeight;
    public Vector3 standCenter;
    public Vector3 standCameraPosition;
    public float crouchHeight;
    public Vector3 crouchCenter;
    public Vector3 crouchCameraPosition;
    public float heightLerpSpeed;
    public float verticalMouseSensitivity;
    public float horizontalMouseSensitivity;
    public float gravity;
    public InputActions actions;
    public float jumpForce;
    public float jumpBuffering;
    public float yThresholdBeforeAudioPlay;

    [Header("Audio")]
    public float walkStepDelay;
    public float runStepDelay;
    public float crouchStepDelay;
    public AudioPlayer audioPlayer;

    private float currentStepDelay;
    private float lastStepTime;

    public EventObject madeQuietNoise;
    public EventObject madeMediumNoise;
    public EventObject madeLoudNoise;

    private Vector2 m_movementDirection;
    private Vector2 m_mouseDelta;
    private float m_gravityForce;
    private float m_lastJumpInput;
    private float m_lastJumpTime;
    private float m_cameraPitch;
    private float m_moveSpeed;

    private EventObject m_currentMoveSound;

    private bool m_running;
    private bool m_crouching;
    private bool m_isGrounded;
    private bool m_landed;
    private bool m_wantsToUnCrouch;
    private bool m_underSomething;
    private float m_yBeforeLanded;
    private bool m_crouchingLastFrame;

    void Start()
    {
        actions = new InputActions();
        actions.Enable();
        m_currentMoveSound = madeMediumNoise;
        m_moveSpeed = walkSpeed;
        currentStepDelay = walkStepDelay;

        actions.Default.Movement.performed += ctx =>
        {
            m_movementDirection = ctx.ReadValue<Vector2>().normalized;
        };
        actions.Default.Look.performed += ctx => m_mouseDelta = ctx.ReadValue<Vector2>();
        actions.Default.Jump.performed += ctx => Jump();
        actions.Default.Run.performed += ctx => SprintStart();
        actions.Default.Run.canceled += ctx => SprintCancelled();

        actions.Default.Crouch.performed += ctx => CrouchStart();
        actions.Default.Crouch.canceled += ctx => CrouchStop();
 
    }
    private void OnDisable() => actions.Disable();

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        m_underSomething = Physics.CheckSphere(headCheck.position, headCheckRadius, groundMask);

        Move();
        
    }
    void Jump()
    {
        if (m_crouching)
            return;

        if (m_isGrounded)
        {
            m_gravityForce = 0f;
            m_gravityForce += jumpForce;
        }
        else
        {
            m_lastJumpInput = jumpForce;
        }

        m_lastJumpTime = Time.time;
    }
    void SprintStart()
    {

        if (m_crouching || !m_isGrounded)
            return;

        m_running = true;
        m_currentMoveSound = madeLoudNoise;
        m_moveSpeed = runSpeed;
        currentStepDelay = runStepDelay;
    }
    void SprintCancelled()
    {
        if (!m_running || m_crouching)
            return;

        m_running = false;
        m_currentMoveSound = madeMediumNoise;
        m_moveSpeed = walkSpeed;
        currentStepDelay = walkStepDelay;
    }

    void CrouchStart()
    {
        if (m_running || !m_isGrounded)
            return;

        m_crouching = true;
        m_currentMoveSound = madeQuietNoise;
        m_moveSpeed = crouchSpeed;
        currentStepDelay = crouchStepDelay;
        audioPlayer.PlayCrouch();
    }

    void CrouchStop()
    {
        if (!m_crouching || m_running)
            return;

        if (m_underSomething)
        {
            m_wantsToUnCrouch = true;
            return;
        }

        m_crouching = false;
        audioPlayer.PlayCrouch();
    }

    void Move()
    {
        if (!m_running)
        {
            if ((!m_underSomething && m_wantsToUnCrouch) || !m_crouching)
            {
                controller.height = standHeight;
                controller.center = standCenter;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, standCameraPosition, Time.deltaTime * heightLerpSpeed);

                m_crouching = false;
                m_wantsToUnCrouch = false;
                m_currentMoveSound = madeMediumNoise;
                m_moveSpeed = walkSpeed;
                if (m_crouchingLastFrame != m_crouching)
                    audioPlayer.PlayCrouch();
            }
            else if (m_crouching)
            {
                controller.height = crouchHeight;
                controller.center = crouchCenter;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, crouchCameraPosition, Time.deltaTime * heightLerpSpeed);
            }
        }

        if (m_running)
        {
            playerCamera.m_Lens.FieldOfView = Mathf.Lerp(playerCamera.m_Lens.FieldOfView, runFOV, Time.deltaTime * fOVLerpSpeed);
        }
        else
        {
            playerCamera.m_Lens.FieldOfView = Mathf.Lerp(playerCamera.m_Lens.FieldOfView, regularFOV, Time.deltaTime * fOVLerpSpeed);
        }

        bool landedFromFall = m_isGrounded && !m_landed;
        if (landedFromFall)
        {
            if ((m_yBeforeLanded - controller.transform.position.y >= yThresholdBeforeAudioPlay) || m_landed)
            {
                madeMediumNoise.Invoke(true);
                audioPlayer.PlayLanded();
            }
        }

        Vector3 moveInput = new Vector3(m_movementDirection.x, 0, m_movementDirection.y);
        if (m_isGrounded)
        {
            if (Time.time - m_lastJumpTime > jumpBuffering)
                m_lastJumpInput = 0.0f;
            else
            {
                if(m_lastJumpInput > 0.0f)
                    audioPlayer.PlayCrouch();
                m_gravityForce += m_lastJumpInput;
            }

            m_yBeforeLanded = controller.transform.position.y;
        }
        else
            m_gravityForce -= gravity * Time.deltaTime;

        moveInput.y += m_gravityForce;

        if ((moveInput.x != 0 || moveInput.z != 0) && m_isGrounded)
        {
            if (Time.time - lastStepTime >= currentStepDelay)
            {
                m_currentMoveSound.Invoke(true);
                audioPlayer.PlayFootstepSound();
                lastStepTime = Time.time;
            }
        }

        controller.Move((moveInput.x * player.right + moveInput.z * player.forward) *
            m_moveSpeed * Time.deltaTime);

        controller.Move(moveInput.y * Vector3.up * Time.deltaTime);

        m_cameraPitch -= m_mouseDelta.y * Time.deltaTime * verticalMouseSensitivity;
        m_cameraPitch = Mathf.Clamp(m_cameraPitch, -90.0f, 90.0f);

        playerCamera.transform.localEulerAngles = new Vector3(m_cameraPitch, 0.0f, 0.0f);
        player.rotation *= Quaternion.Euler(0.0f, m_mouseDelta.x * Time.deltaTime * horizontalMouseSensitivity, 0.0f);
        m_landed = m_isGrounded;
        m_crouchingLastFrame = m_crouching;
    }
}
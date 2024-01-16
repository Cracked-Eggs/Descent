using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2 : MonoBehaviour
{
    public CharacterController controller;
    public Transform player;
    new public Transform camera;
    public float walkSpeed;
    public float runSpeed;
    public float gravity;
    public InputActions actions;
    public float jumpForce;
    public float coyoteTime;

    private Vector2 m_movementDirection;
    private Vector2 m_mouseDelta;
    private float m_gravityForce;
    private float m_lastJumpInput;
    private float m_lastJumpTime;
    private float m_cameraPitch;
    private bool m_running;
    private float m_moveSpeed;

    void Start()
    {
        actions = new InputActions();
        actions.Enable();

        actions.Default.Movement.performed += ctx => m_movementDirection = ctx.ReadValue<Vector2>().normalized;
        actions.Default.Jump.performed += ctx =>
        {
            if (controller.isGrounded)
            {
                m_gravityForce = 0f;
                m_gravityForce += jumpForce;
            }
            else
            {
                m_lastJumpInput = jumpForce;
            }

            m_lastJumpTime = Time.time;
        };
        actions.Default.Run.performed += ctx => m_running = true;
        actions.Default.Run.canceled += ctx => m_running = false;
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Move();

    }

    void Move()
    {
        Vector3 moveInput = new Vector3(m_movementDirection.x, 0, m_movementDirection.y);
        if (controller.isGrounded)
        {
            m_gravityForce = 0.0f;
            if (Time.time - m_lastJumpTime > coyoteTime)
                m_lastJumpInput = 0.0f;
            else
                m_gravityForce += m_lastJumpInput;
        }

        m_gravityForce += !controller.isGrounded ? -gravity * Time.deltaTime : 0;
        moveInput.y += m_gravityForce;

        controller.Move((moveInput.x * player.right + moveInput.z * player.forward) *
            walkSpeed * Time.deltaTime);

        controller.Move(moveInput.y * Vector3.up * Time.deltaTime);
    }
}
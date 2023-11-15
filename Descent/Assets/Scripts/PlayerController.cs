using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float horizontalSensitivity;
    [SerializeField] private float verticalSensitivity;
    [SerializeField] private float gravity;

    private Vector2 moveDirection;
    private float downVelocity;

    void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.y = 1;
        }
        if(Input.GetKey(KeyCode.S)) 
        {
            moveDirection.y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x = -1;
        }

        playerController.Move((player.forward * moveDirection.y + 
            player.right * moveDirection.x).normalized * movementSpeed * Time.deltaTime);

        moveDirection = Vector2.zero;

        downVelocity = !playerController.isGrounded ? downVelocity + gravity * Time.deltaTime : 0;
        playerController.Move(Vector3.down * downVelocity);


        float mouseDeltaX = Input.GetAxis("Mouse X");
        float mouseDeltaY = Input.GetAxis("Mouse Y");

        player.rotation = Quaternion.Euler(player.rotation.eulerAngles + 
            player.up * mouseDeltaX * horizontalSensitivity * Time.deltaTime);

        playerCamera.localRotation *= 
            Quaternion.Euler(Vector3.right * -mouseDeltaY * horizontalSensitivity * Time.deltaTime);
    }
}

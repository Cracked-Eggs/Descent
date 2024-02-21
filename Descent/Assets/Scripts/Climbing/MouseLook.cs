using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [Header("Look Settings")]
    [SerializeField, Range(0, 1)] float lookSpeedX = 2.0f;
    [SerializeField, Range(0, 1)] float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] float upperlookLimit = 80.0f;
    [SerializeField, Range(1, 180)] float lowerlookLimit = 80.0f;
    float _mouseMovementX = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            MouseLooks();
       
    }
    void MouseLooks()
    {
        _mouseMovementX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        _mouseMovementX = Mathf.Clamp(_mouseMovementX, -upperlookLimit, lowerlookLimit);
        virtualCamera.transform.localRotation = Quaternion.Euler(_mouseMovementX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);

    }
}

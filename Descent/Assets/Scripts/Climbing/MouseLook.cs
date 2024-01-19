using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA;
using Unity.VisualScripting;
using static playerPrefss;

public class MouseLook : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] _CharacterController pm;
    private CharacterController controller;

    private InputActions actions;

    private Vector2 inputView;
    Vector3 movementSpeed;

    private Vector3 CameraRot;
    private Vector3 CharacterRot;

    public Transform cameraHolder;

    public float defaultFOV = 60f;
    public float sprintingFOV = 70f;

    public PlayerSettings playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;

    public FreeClimb c;
    public PlayerStateManager p;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Init()
    {
         c = GetComponent<FreeClimb>();
    }

    void Awake()
    {
        actions = new InputActions();
        actions.Enable();

        actions.Default.Look.performed += e => inputView = e.ReadValue<Vector2>();

        controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        MouseLooks();

    }
    void MouseLooks()
    {
        

        CharacterRot.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -inputView.x : inputView.x) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(CharacterRot);

        CameraRot.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted ? inputView.y : -inputView.y) * Time.deltaTime;
        CameraRot.x = Mathf.Clamp(CameraRot.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(CameraRot);
        Debug.Log("Input X: " + inputView.x + ", Input Y: " + inputView.y);
        Debug.Log("CharacterRot: " + CharacterRot + ", CameraRot: " + CameraRot);

    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RopeGrappling : MonoBehaviour
{
    [HideInInspector] public GameObject ropeFreeEnd;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float throwForce;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ropeFreeEnd.GetComponent<Rigidbody>().useGravity = true;
            ropeFreeEnd.GetComponent<Rigidbody>().AddForce(mainCamera.forward * throwForce, ForceMode.Impulse);
            ropeFreeEnd.transform.parent = null;
        }    
    }
}

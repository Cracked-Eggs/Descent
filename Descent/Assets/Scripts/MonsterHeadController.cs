using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHeadController : MonoBehaviour
{
    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private Transform lookAtObj;
    [SerializeField] private float lookAtSpeed;
    [SerializeField] private float pitchDownAmount;
    [SerializeField] private Transform spider;

    void Update()
    {
        lookAtObj.position = Vector3.Lerp(lookAtObj.position, lookAtTarget.position - spider.up * pitchDownAmount, Time.deltaTime * lookAtSpeed);
    }
}

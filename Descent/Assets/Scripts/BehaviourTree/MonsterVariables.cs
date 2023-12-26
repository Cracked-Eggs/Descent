using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterVariables : MonoBehaviour
{
    public float alertness;

    public Vector3 playerLastKnownPos;
    public float alertnessDecreaseRate;

    private void Update()
    {
        alertness = Mathf.Clamp01(alertness);
        alertness -= alertnessDecreaseRate * Time.deltaTime;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterVariables : MonoBehaviour
{
    public float alertness;
    public string playerName;
    public Transform player;
    public Transform spider;

    public Vector3 playerLastKnownPos;
    public float alertnessDecreaseRate;

    private void Start() => player = GameObject.Find(playerName).transform;

    private void Update()
    {
        alertness = Mathf.Clamp01(alertness);
        alertness -= alertnessDecreaseRate * Time.deltaTime;
    }

}

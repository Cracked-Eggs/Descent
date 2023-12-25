using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public EventObject onSpotted;
    public EventObject onPartiallySpotted;
    public Transform eyes;
    public Transform spider;
    public Transform headLook;
    public string playerName;

    public float headRotateSpeed;
    public float headRotateOffset;

    public List<Vision> visions;
    public float alertnessDecreaseRate;

    private Transform m_player;
    private float m_alertness;
    private Vector3 m_playerLastKnownPos;

    void Start()
    {
        m_player = GameObject.Find(playerName).transform;
        m_playerLastKnownPos = eyes.position + eyes.forward * headRotateOffset;
    }

    void Update()
    {
        if (m_alertness > 0.0f)
        {
            headLook.transform.position =
                Vector3.Lerp(headLook.transform.position, m_playerLastKnownPos, Time.deltaTime * headRotateSpeed * m_alertness);
        }
        else
        {
            headLook.transform.position =
                Vector3.Lerp(headLook.transform.position, eyes.position + spider.forward * headRotateOffset, 
                Time.deltaTime * headRotateSpeed);
        }

        foreach (Vision vision in visions) 
        {
            Vector3 toPlayer = m_player.position - eyes.position;

            Vector3 toPlayerXZ = Vector3.ProjectOnPlane(toPlayer, eyes.up);
            Vector3 toPlayerYZ = Vector3.ProjectOnPlane(toPlayer, eyes.right);
            Vector3 forward = (headLook.position - eyes.position).normalized;


            float dpRightXZ = Mathf.Abs(Vector3.Dot(toPlayerXZ.normalized, eyes.right));
            float dpForwardXZ = Mathf.Abs(Vector3.Dot(toPlayerXZ.normalized, eyes.forward));

            float dpUpYZ = Mathf.Abs(Vector3.Dot(toPlayerYZ.normalized, eyes.up));
            float dpForwardYZ = Mathf.Abs(Vector3.Dot(toPlayerYZ.normalized, eyes.forward));

            float distanceXZ = dpForwardXZ * vision.frontDistance + dpRightXZ * vision.sideDistance;
            float distanceYZ = dpForwardYZ * vision.frontDistance + dpUpYZ * vision.sideDistance;

            if (toPlayerXZ.magnitude > distanceXZ || toPlayerYZ.magnitude > distanceYZ)
                continue;

            float dotProductXZ = Vector3.Dot(toPlayerXZ.normalized, Vector3.ProjectOnPlane(forward, eyes.up));
            float dotProductYZ = Vector3.Dot(toPlayerYZ.normalized, Vector3.ProjectOnPlane(forward, eyes.right));

            if (dotProductXZ < vision.coneFOVXZ || dotProductYZ < vision.coneFOVYZ)
                continue;

            RaycastHit hit;
            if (!Physics.Raycast(eyes.position, toPlayer.normalized, out hit))
                continue;

            if (hit.collider.name != playerName)
                continue;

            m_alertness += vision.strength * Time.deltaTime;
            m_playerLastKnownPos = hit.point;
        }

        m_alertness -= Time.deltaTime * alertnessDecreaseRate;
        m_alertness = Mathf.Clamp(m_alertness, 0.0f, 1.0f);

        if(m_alertness == 1.0f)
        {
            onSpotted.Invoke(true);
        }
        else if(m_alertness < 1.0f && m_alertness > 0.5f)
        {
            onPartiallySpotted.Invoke(true);
        }
        else
        {
            onSpotted.Invoke(false);
            onPartiallySpotted.Invoke(false);
        }
    }

    [Header("Debug")]
    public bool visualize;
    public int numLines;
    public float step;

    private void OnDrawGizmos()
    {
        if(!visualize) 
            return;

        int index = 0;

        foreach(Vision vision in visions)
        {
            for (int i = -numLines; i < numLines; i++)
            {
                Vector3 original = eyes.forward;
                Vector3 rotatedXZ = (Quaternion.AngleAxis(step * i, eyes.up) * original).normalized;
                float dpRight = Mathf.Abs(Vector3.Dot(rotatedXZ, eyes.right));
                float dpForward = Mathf.Abs(Vector3.Dot(rotatedXZ, eyes.forward));

                float finalDistance = vision.frontDistance * dpForward + vision.sideDistance * dpRight;

                if (Vector3.Dot(original, rotatedXZ) >= vision.coneFOVXZ)
                {
                    Debug.DrawLine(eyes.position, eyes.position + rotatedXZ * finalDistance, vision.debugColour);
                }

                Vector3 rotatedYZ = (Quaternion.AngleAxis(step * i, eyes.right) * original).normalized;
                float dpRight2 = Mathf.Abs(Vector3.Dot(rotatedYZ, eyes.up));
                float dpForward2 = Mathf.Abs(Vector3.Dot(rotatedYZ, eyes.forward));

                float finalDistance2 = vision.frontDistance * dpForward2 + vision.sideDistance * dpRight2;
                if (Vector3.Dot(original, rotatedYZ) >= vision.coneFOVYZ)
                {
                    Debug.DrawLine(eyes.position, eyes.position + rotatedYZ * finalDistance2, vision.debugColour);
                }
            }

            index++;
        }
    }
}

[System.Serializable]
public struct Vision
{
    [Range(-1.0f, 1.0f)] public float coneFOVXZ;
    [Range(-1.0f, 1.0f)] public float coneFOVYZ;
    public float sideDistance;
    public float frontDistance;
    public float strength;

    [Header("Debug")]
    public Color debugColour;
    public string name;
}

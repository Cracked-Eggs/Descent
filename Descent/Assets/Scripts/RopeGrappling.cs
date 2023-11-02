using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RopeGrappling : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float throwForce;
    [SerializeField] private RopeGenerator ropeSystemPrefab;
    [SerializeField] private Transform ropeEndHolder;
    [SerializeField] private float ropeRange;
    [SerializeField] private int ropeCount;

    private bool hasRope;
    private GameObject currentRopeEnd;
    private List<List<RopeSegment>> ropes;

    private void Start()
    {
        ropes = new List<List<RopeSegment>>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (currentRopeEnd == null)
                return;

            currentRopeEnd.GetComponent<Rigidbody>().useGravity = true;
            currentRopeEnd.GetComponent<Rigidbody>().AddForce(mainCamera.forward * throwForce, ForceMode.Impulse);
            currentRopeEnd.transform.parent = null;
            currentRopeEnd = null;

            hasRope = false;
        }

        if (hasRope)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            int closestRopeIndex = -1;
            float closestRopeRange = Mathf.Infinity;
            int closestRopeSegmentIndex = 0;

            for (int i = 0; i < ropes.Count; i++)
            {
                GameObject startSegment = ropes[i][0].gameObject;
                GameObject endSegment = ropes[i][^1].gameObject;

                float distaceToStartSegment = (startSegment.transform.position - ropeEndHolder.position).magnitude;
                float distanceToEndSegment = (endSegment.transform.position - ropeEndHolder.position).magnitude;

                if (distaceToStartSegment < closestRopeRange)
                {
                    closestRopeRange = distaceToStartSegment;
                    closestRopeIndex = i;
                    closestRopeSegmentIndex = 0;
                }

                if (distanceToEndSegment < closestRopeRange)
                {
                    closestRopeRange = distanceToEndSegment;
                    closestRopeIndex = i;
                    closestRopeSegmentIndex = ropes.Count - 1;
                }
            }

            Debug.Log(closestRopeIndex + ",  " + closestRopeRange);
        }

        if (ropeCount <= 0)
            return;

        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
            {
                if((hit.point - ropeEndHolder.position).magnitude <= ropeRange)
                {
                    hasRope = true;
                    RopeGenerator ropeGenerator = Instantiate(ropeSystemPrefab);

                    List<RopeSegment> rope = ropeGenerator.Initialize(hit.point);

                    rope[^1].gameObject.transform.position = ropeEndHolder.position;
                    rope[^1].gameObject.transform.parent = ropeEndHolder;
                    rope[^1].gameObject.GetComponent<Rigidbody>().useGravity = false;
                    currentRopeEnd = rope[^1].gameObject;

                    ropes.Add(rope);

                    ropeCount--;
                }
            }
        }
    }
}

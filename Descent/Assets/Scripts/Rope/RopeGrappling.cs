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
    [SerializeField] private float ropeClimbRange;
    [SerializeField] private float ropeSamePlaneThreshold;

    [SerializeField] private Transform player;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float climbLerpSpeed;
    [SerializeField] private float climbOffset;
    [SerializeField] private float groundedDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float ropeOffset;

    private bool hasRope;
    private GameObject currentRopeEnd;
    private List<List<RopeSegment>> ropes;

    private bool isClimbing;
    private List<RopeSegment> currentRope;

    private float lerp;

    private void Start()
    {
        ropes = new List<List<RopeSegment>>();
    }

    void Update()
    {
        UpdateOnGround();

        if (Input.GetMouseButton(0))
        {
            if (currentRopeEnd == null)
                return;

            currentRopeEnd.GetComponent<Rigidbody>().useGravity = true;
            currentRopeEnd.GetComponent<Collider>().isTrigger = false;
            currentRopeEnd.GetComponent<Rigidbody>().AddForce(mainCamera.forward * throwForce, ForceMode.Impulse);
            currentRopeEnd.transform.parent = null;
            currentRopeEnd = null;

            hasRope = false;
        }

        if (hasRope)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isClimbing)
                return;

            int closestRopeIndex = -1;
            float closestRopeRange = Mathf.Infinity;
            int closestRopeSegmentIndex = 0;

            for (int i = 0; i < ropes.Count; i++)
            {
                GameObject startSegment = ropes[i][0].gameObject;
                GameObject endSegment = ropes[i][^1].gameObject;

                if (Mathf.Abs(Mathf.Abs(startSegment.transform.position.y) - Mathf.Abs(endSegment.transform.position.y)) < ropeSamePlaneThreshold)
                    continue;

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
                    closestRopeSegmentIndex = ropes[i].Count - 1;
                }
            }

            if (closestRopeRange > ropeClimbRange || closestRopeIndex == -1)
                return;

            int currentValidRopeSegmentIndex = closestRopeSegmentIndex;
            while (ropes[closestRopeIndex][currentValidRopeSegmentIndex].onGround)
            {
                currentValidRopeSegmentIndex += closestRopeSegmentIndex == 0 ? 1 : -1;
            }
            player.transform.position = ropes[closestRopeIndex][currentValidRopeSegmentIndex].gameObject.transform.position;
            
            isClimbing = true;
            lerp = currentValidRopeSegmentIndex / (ropes[closestRopeIndex].Count - 1.0f);
            currentRope = ropes[closestRopeIndex];

            Debug.Log("Attached to rope at " + currentValidRopeSegmentIndex);
        }

        if (isClimbing)
        {
            if (lerp > 1 || lerp < 0)
            {
                lerp = Mathf.Clamp01(lerp);
                player.transform.position =
                    currentRope[Mathf.RoundToInt(lerp * (currentRope.Count - 1))].gameObject.transform.position + 
                    Vector3.up * ropeOffset;

                isClimbing = false;

                Debug.Log("Dropping off rope");
            }

            if (Input.GetKey(KeyCode.S))
                lerp += Time.deltaTime * climbSpeed;

            if (Input.GetKey(KeyCode.W))
                lerp -= Time.deltaTime * climbSpeed;

            Vector3 currentRopeSegmentPos = 
                currentRope[Mathf.RoundToInt(lerp * (currentRope.Count - 1))].gameObject.transform.position;

            player.transform.position = Vector3.Lerp(player.transform.position, currentRopeSegmentPos, 
                Time.deltaTime * climbLerpSpeed);
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

                    List<RopeSegment> rope = ropeGenerator.Initialize(hit.point + hit.normal * 0.1f);

                    rope[^1].gameObject.transform.position = ropeEndHolder.position;
                    rope[^1].gameObject.transform.parent = ropeEndHolder;
                    rope[^1].gameObject.GetComponent<Rigidbody>().useGravity = false;
                    currentRopeEnd = rope[^1].gameObject;

                    currentRopeEnd.GetComponent<Collider>().isTrigger = true;

                    ropes.Add(rope);

                    ropeCount--;
                }
            }
        }
    }

    void UpdateOnGround()
    {
        for (int i = 0; i < ropes.Count; i++)
        {
            List<RopeSegment> currentRope = ropes[i];

            for (int v = 0; v < currentRope.Count; v++)
            {
                RopeSegment currentSegment = currentRope[v];
                currentSegment.onGround = Physics.Raycast(currentSegment.gameObject.transform.position,
                    Vector3.down, groundedDistance, groundLayer);

                currentRope[v] = currentSegment;
            }

            ropes[i] = currentRope;
        }
    }
}

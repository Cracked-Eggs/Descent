using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] LineRenderer lineRenderer;

    public Transform gunTip;
    PlayerMovement playerMovement;


    [Header("Grappling")]
    [SerializeField] float maxGrappleDistance;
    [SerializeField] float grappleDelayTime;
    [SerializeField] float overshootYAxis;

    Vector3 grapplePoint;

    [Header("Cooldown")]
    [SerializeField] float grapplingCd;
    float grapplingCdTimer;

    [Header("Input")]
    [SerializeField] KeyCode grappleKey = KeyCode.Mouse1;

    bool grappling;

    void Start() => playerMovement = GetComponent<PlayerMovement>();

    void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        playerMovement.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    void ExecuteGrapple()
    {
        playerMovement.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        playerMovement.freeze = false;
        grappling = false;
        grapplingCdTimer = grapplingCd;
    }

    public bool IsGrappling() => grappling;

    public Vector3 GetGrapplePoint() => grapplePoint;
}
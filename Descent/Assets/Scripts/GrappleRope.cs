using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Grappling grappling;

    [Header("Settings")]
    [SerializeField] int quality = 200; // how many segments the rope will be split up in
    [SerializeField] float damper = 14; // this slows the simulation down, so that not the entire rope is affected the same
    [SerializeField] float strength = 800; // how hard the simulation tries to get to the target point
    [SerializeField] float velocity = 15; // velocity of the animation
    [SerializeField] float waveCount = 3; // how many waves are being simulated
    [SerializeField] float waveHeight = 1;
    [SerializeField] AnimationCurve affectCurve;

    Spring spring; // returns values for animations
    LineRenderer lr;
    Vector3 currentGrapplePosition;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    void LateUpdate() => DrawRope();

    void DrawRope()
    {
        // if not grappling, don't draw rope
        if (!grappling.IsGrappling())
        {
            currentGrapplePosition = grappling.gunTip.position;

            spring.Reset();

            // reset the positionCount of the lineRenderer
            if (lr.positionCount > 0)
                lr.positionCount = 0;

            return;
        }

        if (lr.positionCount == 0)
        {
            // set the start velocity of the simulation
            spring.SetVelocity(velocity);

            // set the positionCount of the lineRenderer depending on the quality of the rope
            lr.positionCount = quality + 1;
        }

        // set the spring simulation
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = grappling.GetGrapplePoint();
        Vector3 gunTipPosition = grappling.gunTip.position;

        // find the upwards direction relative to the rope
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        // lerp the currentGrapplePositin towards the grapplePoint
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        // loop through all segments of the rope and animate them
        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            // calculate the offset of the current rope segment
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            // lerp the lineRenderer position towards the currentGrapplePosition + the offset you just calculated
            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }
}

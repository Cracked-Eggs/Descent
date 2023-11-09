using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    public float ledgeThreshold = 0.95f; // Adjust this threshold as needed
    public float eyeLevelRaycastDistance = 1.5f; // Adjust the eye level raycast distance
    public float justAboveRaycastDistance = 1.0f; // Adjust the just above raycast distance

    public bool isLedgeDetected = false;

    void Update()
    {
        // Cast two raycasts forward from the player's position
        Vector3 rayOriginEyeLevel = transform.position + Vector3.up * 0.41f; // Eye level starting position
        Vector3 rayDirectionEyeLevel = transform.forward; // Cast horizontally

        Vector3 rayOriginJustAbove = transform.position + Vector3.up * 0.1f; // Just above eye level starting position
        Vector3 rayDirectionJustAbove = transform.forward; // Cast horizontally

        RaycastHit hitEyeLevel, hitJustAbove;

        // Perform the raycasts
        bool hitLedgeEyeLevel = Physics.Raycast(rayOriginEyeLevel, rayDirectionEyeLevel, out hitEyeLevel, eyeLevelRaycastDistance);
        bool hitLedgeJustAbove = Physics.Raycast(rayOriginJustAbove, rayDirectionJustAbove, out hitJustAbove, justAboveRaycastDistance);

        // Check if only the just above raycast hits something indicating a ledge
        isLedgeDetected = hitLedgeJustAbove && !hitLedgeEyeLevel;

        // Debug rays to visualize the raycasts
        Debug.DrawRay(rayOriginEyeLevel, rayDirectionEyeLevel * eyeLevelRaycastDistance, hitLedgeEyeLevel ? Color.green : Color.red);
        Debug.DrawRay(rayOriginJustAbove, rayDirectionJustAbove * justAboveRaycastDistance, hitLedgeJustAbove ? Color.green : Color.red);
    }
}

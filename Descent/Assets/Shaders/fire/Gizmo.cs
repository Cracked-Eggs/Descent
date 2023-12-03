using UnityEngine;

public class BallGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.yellow; // Color of the gizmo wireframe

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Assuming your "Props a flame" script has a public variable named NoiseSize
        PropsAFlame propsScript = GetComponent<PropsAFlame>();

        // Use the NoiseSize value to determine the size of the Gizmo
        float size = propsScript != null ? propsScript.noiseSize : 1.0f;

        // Draw a wire sphere using the adjusted size
        Gizmos.DrawWireSphere(transform.position, size / 2f);
    }
}

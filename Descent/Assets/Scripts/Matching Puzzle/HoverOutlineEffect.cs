using UnityEngine;

public class HoverOutlineEffect : MonoBehaviour
{
    [SerializeField] Material outlineMaterial;
    
    Material originalMaterial; 
    Renderer objectRenderer;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
    }

    void OnMouseEnter() => objectRenderer.material = outlineMaterial;

    void OnMouseExit() => objectRenderer.material = originalMaterial;
}

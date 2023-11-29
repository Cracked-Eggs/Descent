using UnityEngine;
using Cinemachine;

public class MatrixController : MonoBehaviour
{
    private GameObject firstSelectedObject;

    void Update()
    {
        // Check for player input to select or swap game objects
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }

    void HandleSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject selectedObject = hit.transform.gameObject;

            if (firstSelectedObject == null)
            {
                // First click, select the game object
                firstSelectedObject = selectedObject;
            }
            else
            {
                // Second click, swap positions and reset selection
                SwapPosition(firstSelectedObject, selectedObject);
                firstSelectedObject = null;
            }
        }
    }

    void SwapPosition(GameObject obj1, GameObject obj2)
    {
        // Swap positions of the selected objects
        Vector3 tempPosition = obj1.transform.position;
        obj1.transform.position = obj2.transform.position;
        obj2.transform.position = tempPosition;
    }
}

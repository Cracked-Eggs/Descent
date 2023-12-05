using UnityEngine;

public class scrollTest : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectArray = new GameObject[5];
    private int currentPosition = 0;

    void Start()
    {
        SetCurrentObjectActive();
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        // Assuming you want to change the current position using keyboard input
        if (scrollInput > 0)
        {
            currentPosition = (currentPosition + 1) % objectArray.Length;
            SetCurrentObjectActive();
        }
        else if (scrollInput < 0)
        {
            currentPosition = (currentPosition - 1 + objectArray.Length) % objectArray.Length;
            SetCurrentObjectActive();
        }
    }

    void SetCurrentObjectActive()
    {
        // Deactivate all objects
        foreach (GameObject obj in objectArray)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // Activate the current object
        if (objectArray[currentPosition] != null)
        {
            objectArray[currentPosition].SetActive(true);
        }
    }
}

using UnityEngine;
using  SA;
public class scrollTest : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectArray = new GameObject[3];
    private int currentPosition = 0;
    FreeClimb cb;

    float increment;
    float decrease;
    

    void Start()
    {
        SetCurrentObjectActive();
        cb = GetComponent<FreeClimb>();
    }

    void Update()
    {
        if (!cb.isClimbing)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            
            if (scrollInput > 0)
            {
                increment += .5f;
                if (increment > 1f)
                    {
                        currentPosition = (currentPosition + 1) % objectArray.Length;
                        SetCurrentObjectActive();
                        increment = 0f;
                        decrease = 0f;
                    }

            }

            if (scrollInput < 0)
            {
                decrease -= .5f;
                
               
                if(decrease < -1f)
                {
                    currentPosition = (currentPosition - 1 + objectArray.Length) % objectArray.Length;
                    SetCurrentObjectActive();
                    decrease = 0f;
                    decrease = 0f;
                }
                
            }
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

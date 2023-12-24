using UnityEngine;
using  SA;
using JetBrains.Annotations;
using System;

public class scrollTest : MonoBehaviour
{
    [SerializeField]
    
    public GameObject[] objectArray = new GameObject[3];
    public string targetTag = "pickup";
    private int currentPosition = 0;
    FreeClimb cb;
    private GameObject[] adjustedArray;
    public GameObject holder;
    public 
  
    int childCount;

    void Start()
    {
        SetCurrentObjectActive();
        cb = GetComponent<FreeClimb>();
       
    }
    
    void ResizeArray(int newSize)
    {
        adjustedArray = objectArray;
       
    }
    int CountChildObjects(Transform parentTransform)
    {
        
        return parentTransform.childCount;
    }
    
        void Update()
    {
        if (objectArray.Length > 0 && objectArray[0]== null) 
        {
            objectArray = RemoveNullFromArray(objectArray);
        }

        if (holder != null)
        {
            
            childCount = CountChildObjects(holder.transform);
            
        }
        else
        {
            Debug.LogWarning("Target object not assigned in the Inspector.");
        }
        childCount = CountChildObjects(transform);
        childCount = CountChildObjects(holder.transform);
        ResizeArray(childCount);

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject newObject = new GameObject("target object");
            objectArray = AddToArray(adjustedArray, newObject);

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (objectArray.Length > 0 && objectArray[0] != null)
            {
                Destroy(objectArray[0]);
            }
        }

        if (!cb.isClimbing)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            
            if (scrollInput > 0)
            {
               
                        currentPosition = (currentPosition + 1) % objectArray.Length;
                        SetCurrentObjectActive();

            }

            if (scrollInput < 0)
            {

                    currentPosition = (currentPosition - 1 + objectArray.Length) % objectArray.Length;
                    SetCurrentObjectActive();

            }
        }
        
    }
    
    GameObject[] AddToArray(GameObject[] array, GameObject newElement)
    {
        int newSize = array.Length + 1;
        GameObject[] newArray = new GameObject[newSize];

        for (int  i = 0;  i < array.Length;  i++)
        {
            newArray[i] = array[i];
        }
        newArray[newSize - 1] = newElement;

        return newArray;
    }

    GameObject[] RemoveNullFromArray(GameObject[] array)
    {
        // Count the number of non-null elements in the array
        int nonNullCount = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                nonNullCount++;
            }
        }

        // Create a new array with the size of non-null elements
        GameObject[] newArray = new GameObject[nonNullCount];

        // Copy non-null elements from the old array to the new array
        int newArrayIndex = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                newArray[newArrayIndex] = array[i];
                newArrayIndex++;
            }
        }

        return newArray;
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

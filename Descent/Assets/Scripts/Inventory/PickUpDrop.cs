using UnityEngine;

public class PickUpDrop : MonoBehaviour
{
    [SerializeField]
    private GameObject holderSlot;

    private GameObject[] weaponArray;
    private GameObject currentWeapon;

    scrollTest inv;

    void Start()
    {
        scrollTest scrollTest = GetComponent<scrollTest>();
        weaponArray = GetComponent<scrollTest>().objectArray;
        
    }
    bool IsArrayFull(GameObject[] array)
    {
        return array.Length == array.GetLength(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            Drop();
        }
    }

    void PickUp()
    {
        if(!IsArrayFull(weaponArray))
        {
            if (currentWeapon == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 2f); // Adjust the radius as needed

                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("pickup"))
                    {
                        currentWeapon = col.gameObject;
                        currentWeapon.transform.SetParent(holderSlot.transform);
                        currentWeapon.transform.localPosition = Vector3.zero;
                        currentWeapon.SetActive(false);
                        AddToWeaponArray(currentWeapon);
                        break;
                    }
                }
            }
        }
        
    }

    void Drop()
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(true);
            currentWeapon.GetComponent<Rigidbody>().AddForce(transform.forward * 500f); // Adjust force as needed
            currentWeapon.transform.SetParent(null);
            RemoveFromWeaponArray(currentWeapon);
            currentWeapon = null;
        }
    }

    void AddToWeaponArray(GameObject weapon)
    {

       
    }

    void RemoveFromWeaponArray(GameObject weapon)
    {
        
    }
}

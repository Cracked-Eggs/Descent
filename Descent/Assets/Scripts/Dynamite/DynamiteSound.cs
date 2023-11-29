using UnityEngine;
using UnityEngine.Events;

public class DynamiteSound : MonoBehaviour
{
    [SerializeField] UnityEvent _dynamiteSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _dynamiteSound.Invoke();
    }
}

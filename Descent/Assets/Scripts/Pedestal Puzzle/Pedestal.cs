using UnityEngine;
using UnityEngine.Events;

public class Pedestal : MonoBehaviour
{
    [SerializeField] PedestalManager _pedestalManager;
    [SerializeField] UnityEvent _pedestalSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _pedestalManager.PedestalActivated(this);
        _pedestalSound.Invoke();
    }
}
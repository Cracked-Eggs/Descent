using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [SerializeField] PedestalManager _pedestalManager;
    public AudioClip _pedestalSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _pedestalManager.PedestalActivated(this);
    }
}
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip interactSound;
    [SerializeField] AudioSource _audioSource;
    public bool _inRange;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _inRange == true) 
        {
            _audioSource.PlayOneShot(interactSound);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _inRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _inRange = false;
    }
}
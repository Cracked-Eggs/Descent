using UnityEngine;
using UnityEngine.Events;

public class MusicPlayer : MonoBehaviour
{
    public bool _inRange;
    [SerializeField] UnityEvent _playTune;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _inRange == true) 
        {
            _playTune.Invoke();
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
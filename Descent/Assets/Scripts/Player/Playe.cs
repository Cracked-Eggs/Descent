using System;
using UnityEngine;
using UnityEngine.Events;

public class Playe : MonoBehaviour
{
    [SerializeField] GameObject _dynamite;
    [SerializeField] Transform _dynamitePlacement;
    [SerializeField] UnityEvent _startTimerandSound;
    [SerializeField] Camera _cam;

    public int _dynamites { get; private set; }
    public event Action DynamitesChanged;
    public event Action MusicPlayerInteract;
    public event Action MusicPlayerDisable;
    public float interactRange = 5f;


    void Update()
    {
        BlowupDynamite();
    }

    void BlowupDynamite()
    {
        if (_dynamites < 3) return;
        if (_dynamitePlacement == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _startTimerandSound.Invoke();
            var dynamite = Instantiate(_dynamite, _dynamitePlacement.position, transform.rotation);
            Destroy(dynamite, 3f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DynamitePiece"))
        {
            _dynamites++;
            DynamitesChanged?.Invoke();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("MusicPlayer"))
            MusicPlayerInteract.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MusicPlayer"))
            MusicPlayerDisable.Invoke();
    }
}
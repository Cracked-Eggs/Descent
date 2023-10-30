using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject _dynamite;
    [SerializeField] Transform _dynamitePlacement;
    [SerializeField] UnityEvent _startTimer;
    public int _dynamites {  get; private set; }
    public event Action DynamitesChanged;

    void Awake() => FindObjectOfType<PlayerUI>().Bind(this);
    void Update() => BlowupDynamite();

    void BlowupDynamite()
    {
        if (_dynamites < 3) return;
        if (_dynamitePlacement == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _startTimer.Invoke();
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
    }
}
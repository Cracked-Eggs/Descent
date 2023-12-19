using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] UnityEvent _gameEnd;
    [SerializeField] GameObject _dynamite;
    [SerializeField] Transform _dynamitePlacement;
    [SerializeField] Canvas _canvas;
    [SerializeField] UnityEvent _startTimerandSound;
    [SerializeField] UnityEvent _pedestalSound;
    [SerializeField] GameObject _UVFlash;
    [SerializeField] GameObject _flash;
    [SerializeField] Camera _cam;
    [SerializeField] GameObject[] _nums;


    public int _dynamites { get; private set; }
    public int _runes { get; private set; }
    public int _UV {  get; private set; }
    public bool _UVObtained;
    public event Action SafeText;
    public event Action RunesChanged;
    public event Action DynamitesChanged;
    public event Action SafeComplete;
    public event Action PlatesFound;
    public event Action UVComplete;
    public event Action DynamiteInteract;
    public event Action DynamiteInteracted;
    public float interactRange = 5f;

    private bool canInteract = true;
    bool _safeEventTriggered = false;
    bool _plateEventTriggered = false;
    bool _dynamiteInteractTriggered = true;

    void Awake()
    {
        FindObjectOfType<PlayerUI>().Bind(this);
    }


    void Update()
    {

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("GameEnd"))
        {
            _gameEnd.Invoke();
        }

        if (hit.collider.CompareTag("Safe") && _safeEventTriggered == false)
        {
            _safeEventTriggered = true;
            SafeText.Invoke();
        }

        if (hit.collider.CompareTag("UV Num") && _UVFlash.activeSelf)
        {
            foreach (var num in _nums)
                num.gameObject.SetActive(true);
        }
        else
        {
            foreach (var num in _nums)
                num.gameObject.SetActive(false);
        }

        if (hit.collider.CompareTag("PPlates") && _plateEventTriggered == false && _safeEventTriggered == true)
        {
            _plateEventTriggered = true;
            PlatesFound.Invoke();
        }

        if (hit.collider.CompareTag("DynamiteInteract") && _dynamiteInteractTriggered == false)
        {
            _dynamiteInteractTriggered = true;
            DynamiteInteract.Invoke();
        }

        if (hit.collider.CompareTag("Rune"))
        {
            Debug.Log("Hit Rune");
            _runes++;
            Destroy(hit.gameObject);
            RunesChanged.Invoke();
        }

        if (hit.collider.CompareTag("UV"))
        {
            Debug.Log("Hit UV");
            _UV++;
            Destroy(hit.gameObject);
            UVComplete.Invoke();
        }

        if (hit.collider.CompareTag("DynamitePiece"))
        {
            _dynamites++;
            DynamitesChanged?.Invoke();
            _dynamiteInteractTriggered = false;
            Destroy(hit.gameObject);
        }
    }

    IEnumerator Cooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(2f);
        canInteract = true;
    }
}

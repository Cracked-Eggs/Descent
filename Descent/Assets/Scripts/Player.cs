using System;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] UnityEvent _gameEnd;
    [SerializeField] GameObject _dynamite;
    [SerializeField] Transform _dynamitePlacement;
    [SerializeField] Canvas _canvas;
    [SerializeField] UnityEvent _startTimerandSound;
    [SerializeField] PedestalManager _pedestalManager;
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
    AudioManager _audioManager;
    RuneManager _runeMangaer;

    void Awake()
    {
        FindObjectOfType<PlayerUI>().Bind(this);
        _audioManager = FindObjectOfType<AudioManager>();
        _runeMangaer = FindObjectOfType<RuneManager>();
    }


    void Update()
    {
        BlowupDynamite();

        if (Input.GetMouseButtonDown(0))
            TryInteractWithRune();

        if (Input.GetKey(KeyCode.Tab))
            _canvas.gameObject.SetActive(true);
        else
            _canvas.gameObject.SetActive(false);

        if (_runeMangaer._completed == true)
            SafeComplete.Invoke();

        if (_UVObtained == true)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _UVFlash.gameObject.SetActive(true);
                _flash.gameObject.SetActive(false);
                _audioManager.Play("FlashlightOn");
            }
            else
            {
                _UVFlash.gameObject.SetActive(false);
                _flash.gameObject.SetActive(true);
                _audioManager.Play("FlashlightOff");
            }
        }
    }

    void BlowupDynamite()
    {
        if (_dynamites == 0) return;
        if (_dynamitePlacement == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _startTimerandSound.Invoke();
            _audioManager.Play("DynamitePlace");
            var dynamite = Instantiate(_dynamite, _dynamitePlacement.position, transform.rotation);
            DynamiteInteracted.Invoke();
            Destroy(dynamite, 3f);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("GameEnd"))
        {
            StartCoroutine(Monster());
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

        if (hit.collider.CompareTag("Plate"))
        {
            Pedestal pedestal = hit.collider.GetComponent<Pedestal>();

            if (canInteract == true)
            {
                _pedestalManager.PedestalActivated(pedestal);
                _pedestalSound.Invoke();
                StartCoroutine(Cooldown());
            }
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

    void TryInteractWithRune()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            RuneController rune = hit.collider.GetComponent<RuneController>();

            if (rune != null)
            {
                rune.Interact();
            }
        }
    }

    IEnumerator Monster()
    {
        Debug.Log("playing");
        yield return new WaitForSeconds(3f);
        _audioManager.Play("Monster");
    }
}

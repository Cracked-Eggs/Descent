using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] UnityEvent _gameEnd;
    [SerializeField] GameObject _dynamite;
    [SerializeField] Transform _dynamitePlacement;
    [SerializeField] UnityEvent _startTimerandSound;
    [SerializeField] PedestalManager _pedestalManager;
    [SerializeField] UnityEvent _pedestalSound;


    public int _dynamites { get; private set; }
    public int _runes { get; private set; }
    public event Action SafeText;
    public event Action RunesChanged;
    public float interactRange = 5f;

    [SerializeField] Canvas _canvas;
    CharacterController _characterController;
    Pedestal _pedestal;
    private bool canInteract = true;

    void Awake()
    {
        FindObjectOfType<PlayerUI>().Bind(this);
    }

    void Start() => _characterController = GetComponent<CharacterController>();

    void Update()
    {
        BlowupDynamite();

        if (Input.GetKey(KeyCode.Tab))
            _canvas.gameObject.SetActive(true);
        else
            _canvas.gameObject.SetActive(false);
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
            //DynamitesChanged?.Invoke();
            Destroy(other.gameObject);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("GameEnd"))
            _gameEnd.Invoke();

        if (hit.collider.CompareTag("Safe"))
            SafeText.Invoke();

        if (hit.collider.CompareTag("Rune"))
        {
            Debug.Log("Hit Rune");
            _runes++;
            RunesChanged.Invoke();
            Destroy(hit.gameObject);
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
    }

    IEnumerator Cooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(2f);
        canInteract = true;
    }
}

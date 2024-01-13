using UnityEngine;
using UnityEngine.Events;

public class RuneController : MonoBehaviour
{
    public int ID;
    public bool IsMatched;
    [SerializeField] UnityEvent _runeTurnSound;

    RuneManager _runeManager;

    void Start() => _runeManager = FindObjectOfType<RuneManager>();

    public void Interact()
    {
        if (_runeManager != null && !IsMatched)
        {
            _runeManager.OnRuneClicked(this);
            transform.Rotate(Vector3.up, 180f, Space.World);
            IsMatched = true;
            _runeTurnSound.Invoke();
        }

        if (_runeManager._incorrectGuess == true) IsMatched = false;
    }
} 

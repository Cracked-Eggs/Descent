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
            IsMatched = true;
            _runeTurnSound.Invoke();
        }

        if (_runeManager._incorrectGuess == true) IsMatched = false;
    }

    public void SwapPosition(RuneController otherRune)
    {
        Vector3 tempPosition = transform.position;
        transform.position = otherRune.transform.position;
        otherRune.transform.position = tempPosition;
    }
} 

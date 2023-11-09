using UnityEngine;

public class RuneController : MonoBehaviour
{
    public int ID;
    public bool IsMatched;

    RuneManager _runeManager;

    void Start() => _runeManager = FindObjectOfType<RuneManager>();

    public void Interact()
    {
        if (_runeManager != null && !IsMatched)
        {
            _runeManager.OnRuneClicked(this);
            transform.Rotate(0, -180, 0);
            IsMatched = true;
        }

        if (_runeManager._incorrectGuess == true) IsMatched = false;
    }

    void ResetRunes()
    {
    }
} 

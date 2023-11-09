using UnityEngine;

public class RuneController : MonoBehaviour
{
    public int ID;
    public bool IsMatched;

    RuneManager _runeManager;
    Vector3 _startPosition;

    void Start()
    {
        _runeManager = FindObjectOfType<RuneManager>();
        _startPosition = transform.position;
    }

    public void Interact()
    {
        if (_runeManager != null && !IsMatched)
        {
            _runeManager.OnRuneClicked(this);
            transform.Rotate(Vector3.up, 180f, Space.World);
            IsMatched = true;
        }

        if (_runeManager._incorrectGuess == true) IsMatched = false;
    }

    void ResetRunes()
    {
    }
} 

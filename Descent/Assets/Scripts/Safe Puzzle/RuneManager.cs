using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RuneManager : MonoBehaviour
{
    [SerializeField] RuneController[] runes;
    [SerializeField] UnityEvent _puzzleCompleted;
    [SerializeField] UnityEvent _puzzleFailed;
    public bool _incorrectGuess { get; private set; }
    public bool _completed { get; private set; }

    List<RuneController> _allRunes = new List<RuneController>();
    RuneController _selectedRune = null;
    AudioManager _audioManager;

    int _setsMatched = 0;
    int _totalSetsRequired = 2;

    void Awake() => _audioManager = FindObjectOfType<AudioManager>();

    bool CheckMatch(RuneController rune1, RuneController rune2) => rune1.ID == rune2.ID;

    public void OnRuneClicked(RuneController clickedRune)
    {
        if (clickedRune.IsMatched || _setsMatched >= _totalSetsRequired) return;

        if (_selectedRune == null) _selectedRune = clickedRune;
        else
        {
            if (CheckMatch(_selectedRune, clickedRune))
            {
                Debug.Log("Match");
                _allRunes.Add(clickedRune);
                _allRunes.Add(_selectedRune);
                _setsMatched++;
                _selectedRune.IsMatched = true;
                clickedRune.IsMatched = true;

                // Swap positions when there's a match
                _selectedRune.SwapPosition(clickedRune);

                if (_setsMatched == _totalSetsRequired)
                {
                    Debug.Log("All sets matched!");
                    _completed = true;
                    foreach (var rune in _allRunes)
                        Destroy(rune.gameObject, 1f);
                    _audioManager.Play("ShiftComplete");
                    StartCoroutine(PuzzleComplete());
                    _allRunes.Clear();
                }
            }
            else
            {
                StartCoroutine(ResetRunePuzzle());
            }
            _selectedRune = null;
        }
    }

    IEnumerator ResetRunePuzzle()
    {
        Debug.Log("Incorrect!");
        _incorrectGuess = true;
        _puzzleFailed.Invoke();

        yield return new WaitForSeconds(1f);

        foreach (var rune in runes)
        {
            rune.IsMatched = false;
        }
        _allRunes.Clear();
        _setsMatched = 0;
    }

    IEnumerator PuzzleComplete()
    {
        yield return new WaitForSeconds(1.25f);
        _puzzleCompleted.Invoke();
    }
}
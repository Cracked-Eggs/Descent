using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RuneManager : MonoBehaviour
{
    [SerializeField] RuneController[] runes;
    [SerializeField] UnityEvent _puzzleCompleted;
    public bool _incorrectGuess { get; private set; }

    List<RuneController> _allRunes = new List<RuneController>();
    RuneController _selectedRune = null;

    int _setsMatched = 0;
    int _totalSetsRequired = 4;

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

                if (_setsMatched == _totalSetsRequired)
                {
                    Debug.Log("All sets matched!");
                    foreach (var rune in _allRunes)
                        Destroy(rune.gameObject, 0.25f);
                    _puzzleCompleted.Invoke();
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

        yield return new WaitForSeconds(1f);

        foreach (var rune in runes)
        {
            rune.transform.rotation = Quaternion.Euler(0, -90, 0);
            rune.IsMatched = false;
        }
        _allRunes.Clear();
        _setsMatched = 0;
    }
}
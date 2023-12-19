using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TMP_Text _safeText;
    [SerializeField] TMP_Text _runesText;
    [SerializeField] TMP_Text _openText;
    [SerializeField] TMP_Text _platesText;
    [SerializeField] TMP_Text _dynamiteText;
    [SerializeField] TMP_Text _UVText;
    [SerializeField] TMP_Text _pressureText;
    [SerializeField] TMP_Text _interactText;

    [SerializeField] UnityEvent _dynamitePickup;
    [SerializeField] UnityEvent _runesStart;
    [SerializeField] UnityEvent _goal;
    Player _player;



    public void Bind(Player player)
    {
        _player = player;
        _player.SafeText += UpdateSafeText;
        _player.RunesChanged += UpdateRunesText;
        _player.DynamitesChanged += UpdateDynamite;
        _player.SafeComplete += UpdateOpenText;
        _player.UVComplete += UpdateUVText;
        _player.PlatesFound += UpdatePlatesText;
        _player.DynamiteInteract += DynamiteInteract;
        _player.DynamiteInteracted += DynamiteInteracted;
    }

    void UpdateSafeText()
    {
        _safeText.fontStyle = FontStyles.Strikethrough;
        _openText.gameObject.SetActive(true);
        _goal.Invoke();
    }

    void UpdateOpenText()
    {
        _openText.fontStyle = FontStyles.Strikethrough;
        _platesText.gameObject.SetActive(true);
        _player._UVObtained = true;
    }

    void UpdatePlatesText()
    {
        _platesText.fontStyle = FontStyles.Strikethrough;
        _UVText.gameObject.SetActive(true);
        _goal.Invoke();
    }

    void UpdateRunesText()
    {
        var currentRunes = _player._runes;
        _runesText.text = "Find Runes Combination (" + currentRunes + "/4)";
        _goal.Invoke();
        if (currentRunes == 4)
        {
            _runesText.fontStyle = FontStyles.Strikethrough;
            _safeText.gameObject.SetActive(true);
        }
    }

    void UpdateUVText()
    {
        var currentUVs = _player._UV;
        _UVText.text = "Shine UV On Rocks (" + currentUVs + "/4)";
        _goal.Invoke();
        if (currentUVs == 4)
        {
            _UVText.fontStyle = FontStyles.Strikethrough;
            _pressureText.gameObject.SetActive(true);
        }
    }

    void UpdateDynamite()
    {
        _dynamiteText.gameObject.SetActive(true);
        _dynamitePickup.Invoke();
    }

    public void DynamiteStrike() => _dynamiteText.fontStyle = FontStyles.Strikethrough;

    public void UpdatePressureText()
    {
        _pressureText.fontStyle = FontStyles.Strikethrough;
        _goal.Invoke();
    }

    void DynamiteInteract() => _interactText.gameObject.SetActive(true);
    void DynamiteInteracted() => _interactText.gameObject.SetActive(false);
}

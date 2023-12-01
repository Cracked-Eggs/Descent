using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TMP_Text _safeText;
    [SerializeField] TMP_Text _runesText;
    [SerializeField] TMP_Text _openText;
    [SerializeField] TMP_Text _dynamiteText;
    [SerializeField] TMP_Text _UVText;

    [SerializeField] UnityEvent _dynamitePickup;
    [SerializeField] UnityEvent _runesStart;
    Player _player;

    public void Bind(Player player)
    {
        _player = player;
        _player.SafeText += UpdateSafeText;
        _player.RunesChanged += UpdateRunesText;
        _player.DynamitesChanged += UpdateDynamite;
        _player.SafeComplete += UpdateOpenText;
    }

    void UpdateSafeText()
    {
        _safeText.fontStyle = FontStyles.Strikethrough;
        _runesText.gameObject.SetActive(true);
        _runesStart.Invoke();
    }

    void UpdateOpenText()
    {
        _openText.fontStyle = FontStyles.Strikethrough;
        _UVText.gameObject.SetActive(true);
    }

    void UpdateRunesText()
    {
        var currentRunes = _player._runes;
        _runesText.text = "Find Runes for Safe (" + currentRunes + "/4)";
        if (currentRunes == 4)
        {
            _runesText.fontStyle = FontStyles.Strikethrough;
            _openText.gameObject.SetActive(true);
        }
    }

    void UpdateDynamite()
    {
        _dynamiteText.gameObject.SetActive(true);
        _dynamitePickup.Invoke();
    }
}

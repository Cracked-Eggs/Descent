using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TMP_Text _dynamiteText;
    Player _player;

    public void Bind(Player player)
    {
        _player = player;
        _player.DynamitesChanged += UpdateDynamite;
        UpdateDynamite();
    }

    void UpdateDynamite()
    {
        var currentDynamites = _player._dynamites;
        _dynamiteText.text = "Dynamites: " + currentDynamites + "/3";

        if (currentDynamites == 3)
        {
            _dynamiteText.text = "Collected all Dynamites!";
            Destroy(_dynamiteText, 3f);
        }
    }
}

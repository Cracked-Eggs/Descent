using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TMP_Text _dynamiteText;
    [SerializeField] TMP_Text _musicPlayerText;
    Player _player;

    public void Bind(Player player)
    {
        _player = player;
        _player.DynamitesChanged += UpdateDynamite;
        _player.MusicPlayerInteract += ShowMusicInteraction;
        _player.MusicPlayerDisable += DisableMusicInteraction;
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

    void ShowMusicInteraction() => _musicPlayerText.gameObject.SetActive(true);
    void DisableMusicInteraction() => _musicPlayerText.gameObject.SetActive(false);
}

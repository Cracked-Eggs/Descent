using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    public List<SoundLevel> soundLevels;
    public EventObject playerSpotted;
    public float noiseFalloff;
    public MonsterVariables variables;

    private bool m_playerSpotted;

    private void Start()
    {
        playerSpotted.Subscribe((bool spotted) => {m_playerSpotted = spotted;});

        for (int i = 0; i < soundLevels.Count; i++)
        {
            SoundLevel level = soundLevels[i];
            soundLevels[i].soundEvent.Subscribe((bool _) => {
                AlertMonster(level.alertnessLevel, variables.player.position);
                Debug.Log(level.alertnessLevel);
            });
        }
    }

    private void AlertMonster(float amount, Vector3 soundPosition)
    {
        if (m_playerSpotted)
            return;

        float distanceToMonster = (soundPosition - variables.spider.position).magnitude;
        float attentuatedAlertness = amount - distanceToMonster * noiseFalloff;
        if (attentuatedAlertness < 0)
            attentuatedAlertness = 0;

        variables.alertness += attentuatedAlertness;
        variables.playerLastKnownPos = soundPosition;
    }
}

[System.Serializable]
public struct SoundLevel
{
    public float alertnessLevel;
    public EventObject soundEvent;
}

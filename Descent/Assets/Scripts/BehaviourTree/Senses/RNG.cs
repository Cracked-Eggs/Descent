using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG : MonoBehaviour
{
    public List<ProbabilityEvent> probabilityEvents;
    public float rollDelay;

    private bool m_isRolling;

    private void Update()
    {
        if (!m_isRolling)
            StartCoroutine(Roll());
    }

    IEnumerator Roll()
    {
        m_isRolling = true;
        bool invokedPrevious = false;

        for (int i = 0; i < probabilityEvents.Count; i++)
        {
            if (invokedPrevious)
            {
                probabilityEvents[i].eventObject.Invoke(false);
                continue;
            }

            if (probabilityEvents[i].alwaysTrigger)
            {
                probabilityEvents[i].eventObject.Invoke(true);
                break;
            }

            float value = Random.value;
            bool passes = value <= probabilityEvents[i].probability;
            probabilityEvents[i].eventObject.Invoke(passes);
            invokedPrevious = passes;
        }
        yield return new WaitForSeconds(rollDelay);
        m_isRolling = false;
    }
}

[System.Serializable]
public struct ProbabilityEvent
{
    public EventObject eventObject;
    [Range(0.0f, 1.0f)]
    public float probability;
    public bool alwaysTrigger;
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PedestalManager : MonoBehaviour
{
    [SerializeField] List<Pedestal> _pedestals;
    [SerializeField] UnityEvent _puzzleCompleted;
    [SerializeField] UnityEvent _pedestalResetSound;


    List<Pedestal> activatedPedestals = new List<Pedestal>();
    int currentPedestalIndex = 0;

    void Awake()
    {
        activatedPedestals.Clear();
        currentPedestalIndex = 0;
    }

    public void PedestalActivated(Pedestal activatedPedestal)
    {
        if (currentPedestalIndex < _pedestals.Count)
        {
            Pedestal expectedPedestal = _pedestals[currentPedestalIndex];

            if (activatedPedestal == expectedPedestal)
            {
                // correct pedestal activated
                activatedPedestals.Add(activatedPedestal);

                currentPedestalIndex++;

                if (currentPedestalIndex == _pedestals.Count)
                {
                    // puzzle completed
                    foreach (var pedestals in _pedestals)
                        Destroy(pedestals.gameObject);
                    _puzzleCompleted.Invoke();
                }
            }
            else
                ResetPuzzle();
        }
    }

    void ResetPuzzle()
    {
        activatedPedestals.Clear();
        currentPedestalIndex = 0;
        _pedestalResetSound.Invoke();
    }
}

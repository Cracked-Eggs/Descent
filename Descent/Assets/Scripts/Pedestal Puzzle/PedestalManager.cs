﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PedestalManager : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _fullTune;
    [SerializeField] AudioClip _pedestalsReset;
    [SerializeField] List<Pedestal> _pedestals;
    [SerializeField] UnityEvent _puzzleCompleted;


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
                PlaySound(activatedPedestal._pedestalSound);

                currentPedestalIndex++;

                if (currentPedestalIndex == _pedestals.Count)
                {
                    // puzzle completed
                    PlaySound(_fullTune);
                    foreach (var pedestals in _pedestals)
                        Destroy(pedestals.gameObject);
                    _puzzleCompleted.Invoke();
                }
            }
            else
                ResetPuzzle();
        }
    }

    void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    void ResetPuzzle()
    {
        activatedPedestals.Clear();
        currentPedestalIndex = 0;
        PlaySound(_pedestalsReset);
    }
}
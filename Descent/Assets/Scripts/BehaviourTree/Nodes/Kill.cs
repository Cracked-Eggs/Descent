using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Kill : NodeBase
{
    public MonsterVariables variables;
    public Transform newPlayerCamPos;
    public Animator monsterAnimator;
    public string playerCameraName;
    public string flashlightName;
    public float lerpSpeed;
    public float newFlashlightIntensity;
    public string killTrigger;

    private Transform m_player;
    private Transform m_playerCamera;
    private Light m_playerFlashlight;
    private Animator m_cameraAnimator;
    private bool m_isPlaying;

    private bool m_setTrigger;
    private bool m_setCameraTrigger;
    private float m_lerp;

    private void Start()
    {
        m_player = variables.player;
        m_playerCamera = m_player.GetChild(0).Find(playerCameraName);
        m_playerFlashlight = GameObject.Find(flashlightName).GetComponent<Light>();
        m_cameraAnimator = m_playerCamera.GetComponent<Animator>();
    }

    public override void OnTransition()
    {
        m_player.GetComponent<PlayerController>().enabled = false;
        m_isPlaying = true;
    }

    private void Update()
    {
        if (!m_isPlaying)
            return;

        m_playerCamera.position = Vector3.Lerp(m_playerCamera.position,
            newPlayerCamPos.position,
            lerpSpeed * Time.deltaTime);

        m_playerCamera.rotation = Quaternion.Lerp(m_playerCamera.rotation, 
            newPlayerCamPos.rotation, 
            lerpSpeed * Time.deltaTime);

        m_playerFlashlight.intensity = Mathf.Lerp(m_playerFlashlight.intensity, newFlashlightIntensity, Time.deltaTime * lerpSpeed);
        variables.lookAtPos = newPlayerCamPos.position;
        m_lerp += Time.deltaTime * lerpSpeed;

        if (!m_setTrigger)
        {
            m_setTrigger = true;
            monsterAnimator.SetTrigger(killTrigger);
        }

        if(m_lerp >= 1.0f && !m_setCameraTrigger) 
        {
            m_setCameraTrigger = true;
            m_cameraAnimator.enabled = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraControl : MonoBehaviour
{
    public GameObject soundManager;
    private SoundControl soundControl;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundControl = soundManager.GetComponent<SoundControl>();
    }
    public void PlayButtonClick()
    {
        soundControl.PlayButtonSound(audioSource);
    }
}

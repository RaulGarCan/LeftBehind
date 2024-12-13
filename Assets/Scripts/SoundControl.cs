using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    public AudioSource audioSource;
    private AudioClip buttonSound, blobSound, eatSound, healSound, playerHurtSound, footstepsSound, jumpSound, landSound, shootSound, reloadSound, turretSound;
    private AudioClip mainMenuMusic, tutorialMusic, levelMusic;

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonSound);
    }
    public void PlayBlobSound()
    {
        audioSource.PlayOneShot(blobSound);
    }
    public void PlayEatSound()
    {
        audioSource.PlayOneShot(eatSound);
    }
    public void PlayHealSound()
    {
        audioSource.PlayOneShot(healSound);
    }
    public void PlayPlayerHurtSound()
    {
        audioSource.PlayOneShot(playerHurtSound);
    }
    public void PlayFootstepsSound()
    {
        audioSource.PlayOneShot(footstepsSound);
    }
    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }
    public void PlayLandSound()
    {
        audioSource.PlayOneShot(landSound);
    }
    public void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }
    public void PlayTurretSound()
    {
        audioSource.PlayOneShot(turretSound);
    }
    public void PlayReloadSound()
    {
        audioSource.PlayOneShot(reloadSound);
    }
    public void PlayMainMenuMusic()
    {
        audioSource.PlayOneShot(mainMenuMusic);
    }
    public void PlayTutorialMusic()
    {
        audioSource.PlayOneShot(mainMenuMusic);
    }
    public void PlayLevelMusic()
    {
        audioSource.PlayOneShot(mainMenuMusic);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    public AudioClip buttonSound, blobSound, eatSound, healSound, playerHurtSound, footstepsSound, jumpSound, landSound, shootSound, reloadSound, turretSound;

    public void PlayButtonSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(buttonSound);
    }
    public void PlayBlobSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(blobSound);
    }
    public void PlayEatSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(eatSound);
    }
    public void PlayHealSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(healSound);
    }
    public void PlayPlayerHurtSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(playerHurtSound);
    }
    public void PlayFootstepsSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(footstepsSound);
    }
    public void PlayJumpSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(jumpSound);
    }
    public void PlayLandSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(landSound);
    }
    public void PlayShootSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(shootSound);
    }
    public void PlayTurretSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(turretSound);
    }
    public void PlayReloadSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(reloadSound);
    }
}

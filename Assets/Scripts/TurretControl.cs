using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControl : MonoBehaviour
{
    private Animator animator;
    private bool playerTargeted, shotCharged;
    public GameObject bullet;
    private AudioSource audioSource;
    public SoundControl soundControl;
    public GameObject player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(Shoot());
            animator.SetTrigger("Fire");
            playerTargeted = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
        }
    }
    public void ChargeShot()
    {
        shotCharged = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTargeted = false;
            animator.ResetTrigger("Fire");
            StartCoroutine(Shoot());
        }
    }
    IEnumerator Shoot()
    {
        while (true)
        {
            if (playerTargeted && shotCharged)
            {
                soundControl.PlayTurretSound(audioSource);
                TurretShoot();
                Invoke("TurretShoot", 0.2f);
                Invoke("TurretShoot", 0.4f);
                shotCharged = false;
                animator.ResetTrigger("Fire");
                Invoke("SetFireTrigger", 1.5f);
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void SetFireTrigger()
    {
        if (playerTargeted)
        {
            animator.SetTrigger("Fire");
        }
    }
    private void TurretShoot()
    {
        Instantiate(bullet, transform).GetComponent<TurretBulletControl>();
    }
}

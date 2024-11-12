using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    public int playerSpeed;
    public GameObject deathMenu;
    public GameObject bullet;
    public int playerJumpForce;
    public int maxHealth;
    private int health;
    private bool isVulnerable;
    private void Start()
    {
        Time.timeScale = 1f;
        rigPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
        animPlayer = GetComponent<Animator>();
        health = maxHealth;
        isVulnerable = true;
    }
    private void Update()
    {
        FlipSpritePlayer();
        JumpPlayer();
        animPlayer.SetFloat("VelocityAbsX", Math.Abs(rigPlayer.velocity.x));
        animPlayer.SetFloat("VelocityY", rigPlayer.velocity.y);
        ShootPlayer();
    }
    private void FixedUpdate()
    {
        MovementPlayer();
    }
    private void MovementPlayer()
    {
        float inputX = UnityEngine.Input.GetAxis("Horizontal");

        rigPlayer.velocity = new Vector2(inputX * playerSpeed, rigPlayer.velocity.y);
    }
    private void JumpPlayer()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.W) && (TouchFloor()))
        {
            rigPlayer.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);
        }
    }
    private void FlipSpritePlayer()
    {
        if (rigPlayer.velocity.x < 0)
        {
            spritePlayer.flipX = true;
        }
        else if (rigPlayer.velocity.x > 0)
        {
            spritePlayer.flipX = false;
        }
    }
    private bool TouchFloor()
    {
        RaycastHit2D touch = Physics2D.Raycast(transform.position + new Vector3(0, -1.6f, 0), Vector2.down, 0.2f);

        if (touch.collider != null && touch.collider.CompareTag("Ground"))
        {
            return true;
        }
        return false;
    }
    public void HurtPlayer(int damage)
    {
        if (isVulnerable)
        {
            Debug.Log("PlayerHealth: "+health);
            health-=damage;
            MakeInvunerable();
            Invoke("MakeVulnerable", 1f);
            if (health<=0)
            {
                DeathPlayer();
            }
        }
    }
    private void MakeInvunerable()
    {
        spritePlayer.color = Color.red;
        isVulnerable = false;
    }
    private void MakeVulnerable()
    {
        spritePlayer.color = Color.white;
        isVulnerable = true;
    }
    private void DeathPlayer()
    {
        Time.timeScale = 0f;
        deathMenu.SetActive(true);
    }
    private void ShootPlayer()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bullet, transform);
        }
    }
}

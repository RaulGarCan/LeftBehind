using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigPlayer;
    private SpriteRenderer spritePlayer;
    public int playerSpeed;
    public int playerJumpForce;
    private void Start()
    {
        rigPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        FlipSpritePlayer();
        JumpPlayer();
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
}

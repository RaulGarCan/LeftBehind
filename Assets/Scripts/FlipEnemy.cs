using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipEnemy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float lastPositionX;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPositionX = transform.parent.position.x;
    }
    private void Update()
    {
        if (lastPositionX - transform.parent.position.x > 0) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;

        lastPositionX = transform.parent.position.x;
    }
}

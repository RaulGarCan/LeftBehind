using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyBlobControl : MonoBehaviour
{
    private GameObject player;
    private float chaseSpeed, startYPos;
    private void Start()
    {
        chaseSpeed = GetComponent<BlobControl>().speed;
        startYPos = transform.position.y;
    }
    private void Update()
    {
        if (!GetComponent<SpriteRenderer>().flipX)
        {
            LookForPlayerLeft();
        }
        else
        {
            LookForPlayerRight();
        }
        if (GetComponent<BlobControl>().isChasing)
        {
            ChasePlayer();
        }
    }
    private void FixedUpdate()
    {
        if (GetComponent<BlobControl>().isChasing && chaseSpeed < GetComponent<BlobControl>().speed*3)
        {
            chaseSpeed += 0.01f;
        }
    }
    private void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x,startYPos, 0f), chaseSpeed * Time.deltaTime);
    }
    private void LookForPlayerLeft()
    {
        if (GetComponent<BlobControl>().isChasing)
        {
            return;
        }
        RaycastHit2D left = Physics2D.Raycast(transform.position + new Vector3(-2f, 0, 0), Vector2.left, 1f);
        if (left.collider != null)
        {
            if (left.collider.CompareTag("Player"))
            {
                Debug.Log("PlayerLeft");
                player = left.collider.gameObject;
                GetComponent<BlobControl>().isChasing = true;
            }
        }
    }
    private void LookForPlayerRight()
    {
        if (GetComponent<BlobControl>().isChasing)
        {
            return;
        }
        RaycastHit2D right = Physics2D.Raycast(transform.position + new Vector3(2f, 0, 0), Vector2.right, 1f);
        if (right.collider != null)
        {
            if (right.collider.CompareTag("Player"))
            {
                Debug.Log("PlayerRight");
                player = right.collider.gameObject;
                GetComponent<BlobControl>().isChasing = true;
            }
        }
    }
}

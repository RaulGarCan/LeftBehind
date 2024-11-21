using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlobControl : MonoBehaviour
{
    private bool movingToEndPos;
    private Vector3 startPos, endPos;
    public int finalPosX, speed, damage, radAmount, maxHealth;
    public GameObject blobHealthBar;
    private Image imageHealthBar;
    private void Start()
    {
        startPos = transform.position;
        movingToEndPos = true;
        endPos = new Vector3(finalPosX, transform.position.y, transform.position.z);
        blobHealthBar.SetActive(false);
    }
    private void Update()
    {
        if (movingToEndPos)
        {
            MoveToPos(transform.position, endPos);
        } else
        {
            MoveToPos(transform.position, startPos);
        }
    }
    private void MoveToPos(Vector3 startPos, Vector3 finalPos)
    {
        transform.position = Vector3.MoveTowards(startPos, finalPos, speed * Time.deltaTime);
        if (transform.position == finalPos)
        {
            movingToEndPos = !movingToEndPos;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().HurtPlayer(damage, radAmount);
        }
    }
}

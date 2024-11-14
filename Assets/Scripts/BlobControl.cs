using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobControl : MonoBehaviour
{
    public int speed;
    private bool movingToEndPos;
    public int finalPosX;
    private Vector3 startPos;
    private Vector3 endPos;
    public int damage, radAmount;
    private void Start()
    {
        startPos = transform.position;
        movingToEndPos = true;
        endPos = new Vector3(finalPosX, transform.position.y, transform.position.z);
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

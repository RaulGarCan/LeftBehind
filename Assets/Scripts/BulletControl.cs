using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float speed;
    private GameObject player;
    private Vector3 endPos, bulletDir;
    private void Start()
    {
        player = transform.parent.gameObject;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y+0.5f, player.transform.position.z);
        bulletDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPos = bulletDir;
        Debug.Log("EndPos: " + endPos);
        if (player.GetComponent<SpriteRenderer>().flipX) transform.position = new Vector3(transform.position.x-1, transform.position.y, transform.position.z);
        else transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

        Invoke("DestroyBullet", 5f);
    }
    private void Update()
    {
        MoveBullet();
        //Debug.Log("BulletPos: "+transform.position);
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void MoveBullet()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            DestroyBullet();
        }
    }
}

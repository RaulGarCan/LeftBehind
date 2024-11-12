using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float speed;
    private GameObject player;
    private Vector3 endPos;
    private void Start()
    {
        player = transform.parent.gameObject;
        transform.position = player.transform.position;
        endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("EndPos: " + endPos);
        if (player.GetComponent<SpriteRenderer>().flipX) transform.position.Set(transform.position.x+5, transform.position.y, transform.position.z);
        else transform.position.Set(transform.position.x - 5, transform.position.y, transform.position.z);

        Invoke("DestroyBullet", 5f);
    }
    private void Update()
    {
        MoveBullet();
        Debug.Log("BulletPos: "+transform.position);
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
        DestroyBullet();
    }
}

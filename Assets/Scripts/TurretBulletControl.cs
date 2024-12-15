using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBulletControl : MonoBehaviour
{
    public int speed;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector3 endPos;
    private void Awake()
    {
        transform.position = transform.parent.position;
        player = transform.parent.GetComponent<TurretControl>().player;
        MoveTurretBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().HurtPlayer(15,0,30,30);
        }
        if (!collision.CompareTag("Item") && !collision.CompareTag("Void") && !collision.CompareTag("Turret") && !collision.CompareTag("Bullet"))
        {
            DestroyBullet();
        }
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void MoveTurretBullet()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(transform.parent.position.x, transform.parent.transform.position.y + 0.5f, transform.parent.transform.position.z);
        endPos = player.transform.position;

        if (transform.parent.GetComponent<SpriteRenderer>().flipX) transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        else transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

        Vector3 direction = player.transform.position - transform.position;
        Vector3 rotation = transform.position - player.transform.position;
        float rot = Mathf.Atan2(rotation.x, rotation.y) * Mathf.Rad2Deg;
        if (!transform.parent.GetComponent<SpriteRenderer>().flipX)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Abs(rot) + 90);
        }
        else
        {
            rot -= 180;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Abs(rot) - 90);
        }
        rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;

        Invoke("DestroyBullet", 5f);
    }
}

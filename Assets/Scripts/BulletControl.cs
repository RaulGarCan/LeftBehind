using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float speed;
    private GameObject player;
    private Vector3 endPos, bulletDir;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = transform.parent.gameObject;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y+0.5f, player.transform.position.z);
        bulletDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPos = bulletDir;
        Debug.Log("EndPos: " + endPos);
        
        if (player.GetComponent<SpriteRenderer>().flipX) transform.position = new Vector3(transform.position.x-1, transform.position.y, transform.position.z);
        else transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        float rot = Mathf.Atan2(rotation.x, rotation.y) * Mathf.Rad2Deg;
        Debug.Log("MouseRot"+ rot);
        if (!player.GetComponent<SpriteRenderer>().flipX)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Abs(rot) + 90);
        } else
        {
            rot -= 180;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Abs(rot) - 90);
        }
        rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;

        Invoke("DestroyBullet", 5f);
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
        if (collision.gameObject.CompareTag("Blob"))
        {
            collision.gameObject.GetComponent<BlobControl>().HurtBlob();
        }
        if (!collision.CompareTag("Player") && !collision.CompareTag("Item"))
        {
            DestroyBullet();
        }
    }
}

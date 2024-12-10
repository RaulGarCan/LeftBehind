using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidControl : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().HurtFallPlayer();
        }
    }
}

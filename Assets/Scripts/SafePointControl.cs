using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePointControl : MonoBehaviour
{
    private GameObject player;
    private void Awake()
    {
        player = transform.parent.GetComponent<VoidControl>().player;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().lastGroundPoint = player.transform.position;
        }
    }
}

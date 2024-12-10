using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePointControl : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().lastGroundPoint = player.transform.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoaderControl : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GetComponent<PlayerControl>().ExitTutorial();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderControl : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().name.ToLower().Contains("tutorial"))
            {
                player.GetComponent<PlayerControl>().ExitTutorial();
            } else if (!SceneManager.GetActiveScene().name.ToLower().Contains("level2"))
            {
                player.GetComponent<PlayerControl>().NextLevel();
            } else
            {
                player.GetComponent<PlayerControl>().FinalLevel();
            }
        }
    }
}

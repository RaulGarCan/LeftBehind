using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPersist : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("MenuMusic").Length>1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void StopMusic()
    {
        Destroy(gameObject);
    }
}

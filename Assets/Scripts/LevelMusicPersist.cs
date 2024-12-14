using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicPersist : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("LevelMusic").Length > 1)
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

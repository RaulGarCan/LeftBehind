using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalLevelControl : MonoBehaviour
{
    public GameObject scoreText, itemsText, timeText, ammoText, enemiesText, hitText;
    private int itemsCollected, ammoUsed, enemiesKilled, hitCount, score;
    private float totalTime;
    private void Awake()
    {
        LoadDataFromPersist();
        scoreText.GetComponent<TMP_Text>().text = score.ToString();
        itemsText.GetComponent<TMP_Text>().text = itemsCollected.ToString();
        ammoText.GetComponent<TMP_Text>().text = ammoUsed.ToString();
        enemiesText.GetComponent<TMP_Text>().text = enemiesKilled.ToString();
        hitText.GetComponent<TMP_Text>().text = hitCount.ToString();
        timeText.GetComponent<TMP_Text>().text = ((int)totalTime/60).ToString("00")+":"+ ((int)totalTime % 60).ToString("00");
    }
    private void LoadDataFromPersist()
    {
        itemsCollected = PlayerPrefs.GetInt("itemsCollected");
        ammoUsed = PlayerPrefs.GetInt("ammoUsed");
        enemiesKilled = PlayerPrefs.GetInt("enemiesKilled");
        hitCount = PlayerPrefs.GetInt("hitCount");
        totalTime = PlayerPrefs.GetFloat("totalTime");
        score = PlayerPrefs.GetInt("scorePlayer");
    }
}

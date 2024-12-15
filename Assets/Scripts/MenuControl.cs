using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    private GameDifficulty gameDifficulty;
    private DifficultyControl difficultyControl;
    public GameObject sceneFader;
    private string sceneName;
    private bool isFading;
    public enum GameDifficulty
    {
        easy,
        normal,
        hard
    }
    private void Start()
    {
        isFading = false;
        Time.timeScale = 1;
        difficultyControl = GetComponent<DifficultyControl>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.ToLower().Contains("level") || SceneManager.GetActiveScene().name.ToLower().Equals("tutorial"))
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !SceneManager.GetActiveScene().name.ToLower().Equals("mainmenu") && !isFading)
        {
            FadeScene("MainMenu");
        }
    }
    public void FadeGameScene(string sceneName)
    {
        isFading = true;
        Time.timeScale = 1f;
        this.sceneName = sceneName;
        FadeAnimation();
        Invoke("LoadGame", 1f);
    }
    public void Retry(string sceneName)
    {
        isFading = true;
        Time.timeScale = 1f;
        this.sceneName = sceneName;
        FadeAnimation();
        Invoke("LoadScene", 1f);
    }
    public void SetDifficulty(string dificultyName)
    {
        switch (dificultyName.ToLower())
        {
            case "easy":
                gameDifficulty = GameDifficulty.easy;
                break;
            case "normal":
                gameDifficulty = GameDifficulty.normal;
                break;
            case "hard":
                gameDifficulty = GameDifficulty.hard;
                break;
            default:
                break;
        }
    }
    public void FadeScene(string sceneName)
    {
        isFading = true;
        Time.timeScale = 1;
        this.sceneName = sceneName;
        FadeAnimation();
        Invoke("LoadScene", 1f);
    }
    private void FadeAnimation()
    {
        sceneFader.GetComponent<Animator>().SetTrigger("FadeOut");
    }
    private void LoadGame()
    {
        difficultyControl.StoreDifficultyData(gameDifficulty);
        SceneManager.LoadScene(sceneName);
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

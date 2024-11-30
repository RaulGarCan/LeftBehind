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
    public enum GameDifficulty
    {
        easy,
        normal,
        hard
    }
    private void Start()
    {
        Time.timeScale = 1;
        difficultyControl = GetComponent<DifficultyControl>();
    }
    public void FadeGameScene(string sceneName)
    {
        this.sceneName = sceneName;
        FadeAnimation();
        Invoke("LoadGame", 1f);
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
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

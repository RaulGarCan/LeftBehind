using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    private GameDifficulty gameDifficulty;
    enum GameDifficulty
    {
        EASY,
        NORMAL,
        HARD
    }
    // MainMenu Control
    public void ExitGame()
    {
        Application.Quit();
    }
    public void GoToDifficultyScene()
    {
        SceneManager.LoadScene("DifficultyMenu");
    }
    public void GoToCreditsScene()
    {
        SceneManager.LoadScene("CreditsMenu");
    }
    // CreditsMenu Control
    public void GoToMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // DifficultyMenu Control
    public void GoToGameEasyDifficulty()
    {
        LoadGame(GameDifficulty.EASY);
    }
    public void GoToGameNormalDifficulty()
    {
        LoadGame(GameDifficulty.NORMAL);
    }
    public void GoToGameHardDifficulty()
    {
        LoadGame(GameDifficulty.HARD);
    }
    private void LoadGame(GameDifficulty gameDifficulty)
    {
        SceneManager.LoadScene("Demo");
    }
}

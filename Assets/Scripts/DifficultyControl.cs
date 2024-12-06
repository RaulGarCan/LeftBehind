using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuControl;

public class DifficultyControl : MonoBehaviour
{
    public void StoreDifficultyData(GameDifficulty gameDifficulty)
    {
        DifficultySettings difficultySettings = DifficultySettings.SetDifficultySettings(gameDifficulty);
        PlayerPrefs.SetFloat("difficultyEnemyDamage", difficultySettings.enemyDamageMultiplier);
        PlayerPrefs.SetFloat("difficultyEnemyHealth", difficultySettings.enemyHealthMultiplier);
        PlayerPrefs.SetFloat("difficultyPlayerHealth", difficultySettings.playerHealthMultiplier);
    }
    class DifficultySettings
    {
        public float enemyDamageMultiplier;
        public float enemyHealthMultiplier;
        public float playerHealthMultiplier;
        DifficultySettings(float enemyDamage, float enemyHealth, float playerHealth)
        {
            enemyDamageMultiplier = enemyDamage;
            enemyHealthMultiplier = enemyHealth;
            playerHealthMultiplier = playerHealth;
        }
        public static DifficultySettings SetDifficultySettings(GameDifficulty gameDifficulty)
        {
            switch (gameDifficulty)
            {
                case GameDifficulty.easy:
                    return new DifficultySettings(0.75f, 0.75f, 1.25f);
                case GameDifficulty.normal:
                    return new DifficultySettings(1f, 1f, 1f);
                case GameDifficulty.hard:
                    return new DifficultySettings(1.25f, 1.25f, 0.75f);
                default:
                    return null;
            }
        }
    }
    public float GetEnemyDamageMultiplier()
    {
        return PlayerPrefs.GetFloat("difficultyEnemyDamage");
    }
    public float GetEnemyHealthMultiplier()
    {
        return PlayerPrefs.GetFloat("difficultyEnemyHealth");
    }
    public float GetPlayerHealthMultiplier()
    {
        return PlayerPrefs.GetFloat("difficultyPlayerHealth");
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject hungerHUD, healthHUD, ammoHUD, remAmmoHUD, radHUD, timerHUD, medkitHUD, foodHUD, scoreHUD, pauseMenuCanvas, staminaHUD, bleedIcon, fractureIcon;
    private TMP_Text ammoText, remAmmoText, timerText, itemMedkitText, itemFoodText, scoreText;
    private void Start()
    {
        ammoText = ammoHUD.GetComponent<TMP_Text>();
        remAmmoText = remAmmoHUD.GetComponent<TMP_Text>();
        timerText = timerHUD.GetComponent<TMP_Text>();
        itemMedkitText = medkitHUD.GetComponent<TMP_Text>();
        itemFoodText = foodHUD.GetComponent<TMP_Text>();
        scoreText = scoreHUD.GetComponent<TMP_Text>();
    }
    public void SetHungerHUD(int hunger, int maxHunger)
    {
        hungerHUD.GetComponent<Image>().fillAmount = (float)hunger/maxHunger;
    }
    public void SetHealthHUD(int health, int maxHealth)
    {
        healthHUD.GetComponent<Image>().fillAmount = (float)health/maxHealth;
    }
    public void SetRadHUD(int rad, int maxRad)
    {
        radHUD.GetComponent<Image>().fillAmount = (float)rad/maxRad;
    }
    public void SetAmmoHUD(string ammo)
    {
        ammoText.text = ammo;
    }
    public void SetRemAmmoHUD(int remAmmo) 
    { 
        remAmmoText.text = remAmmo.ToString();
    }
    public void SetTimerHUD(int min, int sec){
        timerText.text = min.ToString("00") +":"+ sec.ToString("00");
    }
    public void SetMedkitHUD(int amount)
    {
        itemMedkitText.text = amount.ToString();
    }
    public void SetFoodHUD(int amount)
    {
        itemFoodText.text = amount.ToString();
    }
    public void SetScoreHUD(int score)
    {
        scoreText.text = score.ToString();
    }
    public void SetStaminaHUD(float stamina, float maxStamina)
    {
        staminaHUD.GetComponent<Image>().fillAmount = (float)stamina / maxStamina;
    }
    public void PauseMenu()
    {
        if (!pauseMenuCanvas.activeSelf)
        {
            pauseMenuCanvas.SetActive(true);
            Invoke("Pause", 0.001f);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuCanvas.SetActive(false);
        }
    }
    private void Pause()
    {
        Time.timeScale = 0f;
    }
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        pauseMenuCanvas.SetActive(false);
    }
    public void EnableBleed()
    {
        bleedIcon.SetActive(true);
    }
    public void DisableBleed()
    {
        bleedIcon.SetActive(false);
    }
    public void EnableFracture()
    {
        fractureIcon.SetActive(true);
    }
    public void DisableFracture()
    {
        fractureIcon.SetActive(false);
    }
}

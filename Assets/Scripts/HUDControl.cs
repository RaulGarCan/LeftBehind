using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject hungerHUD, healthHUD, ammoHUD, remAmmoHUD, radHUD, timerHUD;
    private TMP_Text ammoText, remAmmoText, timerText;
    private void Start()
    {
        ammoText = ammoHUD.GetComponent<TMP_Text>();
        remAmmoText = remAmmoHUD.GetComponent<TMP_Text>();
        timerText = timerHUD.GetComponent<TMP_Text>();
    }
    public void SetHungerHUD(int hunger)
    {
        hungerHUD.GetComponent<Image>().fillAmount = (float)hunger/100;
    }
    public void SetHealthHUD(int health)
    {
        healthHUD.GetComponent<Image>().fillAmount = (float)health/100;
    }
    public void SetRadHUD(int rad)
    {
        radHUD.GetComponent<Image>().fillAmount = (float)rad/100;
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
}

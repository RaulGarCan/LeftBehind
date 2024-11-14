using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDControl : MonoBehaviour
{
    public GameObject hungerHUD, healthHUD, ammoHUD, remAmmoHUD, radHUD;
    private TMP_Text hungerText, healthText, ammoText, remAmmoText, radText;
    private void Start()
    {
        hungerText = hungerHUD.GetComponent<TMP_Text>();
        healthText = healthHUD.GetComponent<TMP_Text>();
        ammoText = ammoHUD.GetComponent<TMP_Text>();
        remAmmoText = remAmmoHUD.GetComponent<TMP_Text>();
        radText = radHUD.GetComponent<TMP_Text>();
    }
    public void SetHungerHUD(int hunger)
    {
        hungerText.text = hunger.ToString();
    }
    public void SetHealthHUD(int health)
    {
        healthText.text = health.ToString();
    }
    public void SetAmmoHUD(int ammo)
    {
        ammoText.text = ammo.ToString();
    }
    public void SetRemAmmoHUD(int remAmmo) 
    { 
        remAmmoText.text = remAmmo.ToString();
    }
    public void SetRadHUD(int rad)
    {
        radText.text = rad.ToString();
    }
}

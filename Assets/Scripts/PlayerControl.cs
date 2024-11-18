using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    public GameObject deathMenu, bullet, HUD;
    private HUDControl hudControl;
    public int playerJumpForce, playerSpeed, maxHealth, maxHunger, maxRadiation, maxAmmo, maxMagAmmo;
    private int health, hunger, radiation, remainingAmmo, magazineAmmo, playerSpeedOrig, hungerMultiplier, itemMedkit, itemFood, scorePlayer;
    private bool isVulnerable, isReloading, isHungry, isRadiating;
    private float lastTimeShoot, lastTimeHunger, lastTimeRad;
    private string ammoString;
    private void Start()
    {
        Time.timeScale = 1f;
        rigPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
        animPlayer = GetComponent<Animator>();
        health = maxHealth;
        hunger = maxHunger;
        radiation = maxRadiation;
        remainingAmmo = maxAmmo;
        magazineAmmo = maxMagAmmo;
        isVulnerable = true;
        hudControl = HUD.GetComponent<HUDControl>();
        lastTimeShoot = Time.time;
        isReloading = false;
        playerSpeedOrig = playerSpeed;
        ammoString = magazineAmmo.ToString();
        hungerMultiplier = 5;
        isHungry = true;
        itemMedkit = 0;
        itemFood = 0;

        UpdateHUDInfo();
    }
    private void Update()
    {
        FlipSpritePlayer();
        JumpPlayer();
        animPlayer.SetFloat("VelocityAbsX", Math.Abs(rigPlayer.velocity.x));
        animPlayer.SetFloat("VelocityY", rigPlayer.velocity.y);
        ShootPlayer();
        ReloadGunPlayer();
        UpdateHUDInfo();
        if (radiation<=0 && Time.time>lastTimeRad + 1f)
        {
            lastTimeRad = Time.time;
            ReduceHealthPlayer();
        }
        if (hunger<=0 && Time.time > lastTimeHunger + 1f)
        {
            ReduceHealthPlayer();
        }
        if(isHungry){
            isHungry = false;
            Invoke("ReduceHungerPlayer", 1f);
        }
        EatFoodPlayer();
        UseMedKitPlayer();
    }
    private void FixedUpdate()
    {
        MovementPlayer();
    }
    private void MovementPlayer()
    {
        float inputX = UnityEngine.Input.GetAxis("Horizontal");

        rigPlayer.velocity = new Vector2(inputX * playerSpeed, rigPlayer.velocity.y);
    }
    private void JumpPlayer()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.W) && TouchFloor() && !isReloading)
        {
            rigPlayer.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);
        }
    }
    private void FlipSpritePlayer()
    {
        if (rigPlayer.velocity.x < 0)
        {
            spritePlayer.flipX = true;
        }
        else if (rigPlayer.velocity.x > 0)
        {
            spritePlayer.flipX = false;
        }
    }
    private bool TouchFloor()
    {
        RaycastHit2D touch = Physics2D.Raycast(transform.position + new Vector3(0, -1.6f, 0), Vector2.down, 0.2f);

        if (touch.collider != null && touch.collider.CompareTag("Ground"))
        {
            return true;
        }
        return false;
    }
    public void HurtPlayer(int damage, int rad)
    {
        if (isVulnerable)
        {
            ReduceRadiationPlayer(rad);
            Debug.Log("PlayerHealth: "+health);
            health-=damage;
            MakeInvunerable();
            Invoke("MakeVulnerable", 4f);
            if (health<=0)
            {
                DeathPlayer();
            }
        }
    }
    private void MakeInvunerable()
    {
        spritePlayer.color = Color.red;
        isVulnerable = false;
    }
    private void MakeVulnerable()
    {
        spritePlayer.color = Color.white;
        isVulnerable = true;
    }
    private void DeathPlayer()
    {
        Time.timeScale = 0f;
        deathMenu.SetActive(true);
    }
    private void ShootPlayer()
    {
        if (TouchFloor() && UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) && magazineAmmo>0 && Time.time > lastTimeShoot+0.3f && !isReloading)
        {
            Instantiate(bullet, transform);
            magazineAmmo--;
            lastTimeShoot = Time.time;
            ammoString = magazineAmmo.ToString();
        }
    }
    private void ReloadGunPlayer()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && magazineAmmo<maxMagAmmo && !isReloading && remainingAmmo>0 && TouchFloor())
        {
            isReloading = true;
            playerSpeed /= 2;
            ammoString = "??";
            Invoke("RefillGun",2.5f);
        }
    }
    public void RefillGun(){
        int neededAmmo = maxMagAmmo-magazineAmmo;
        isReloading = false;
        playerSpeed = playerSpeedOrig;

        if(remainingAmmo>=neededAmmo)
        {
            magazineAmmo += neededAmmo;
            remainingAmmo-= neededAmmo;
        } else {
            magazineAmmo += remainingAmmo;
            remainingAmmo = 0;
        }

        ammoString = magazineAmmo.ToString();
    }
    private void UpdateHUDInfo()
    {
        hudControl.SetAmmoHUD(ammoString);
        hudControl.SetHealthHUD(health);
        hudControl.SetHungerHUD(hunger);
        hudControl.SetRemAmmoHUD(remainingAmmo);
        hudControl.SetRadHUD(radiation);
        hudControl.SetTimerHUD((int)Time.time/60, (int)Time.time%60);
        hudControl.SetFoodHUD(itemFood);
        hudControl.SetMedkitHUD(itemMedkit);
        hudControl.SetScoreHUD(scorePlayer);
    }
    public void ReduceRadiationPlayer(int radAmount)
    {
        if (radiation-radAmount>0)
        {
            radiation -= radAmount;
        } else
        {
            radiation = 0;
        }
    }
    public void ReduceHungerPlayer()
    {
        isHungry = true;
        if (hunger-hungerMultiplier>0)
        {
            hunger -= hungerMultiplier;
        } else
        {
            hunger = 0;
        }
        lastTimeHunger = Time.time;
    }
    private void ReduceHealthPlayer()
    {
        health--;
    }
    public void PickupFoodCan(int addScore)
    {
        itemFood++;
        scorePlayer += addScore;
    }
    public void PickupMedKit(int addScore)
    {
        itemMedkit++;
        scorePlayer += addScore;
    }
    public void PickupAmmo(int addAmmo)
    {
        if (remainingAmmo+addAmmo > maxAmmo)
        {
            remainingAmmo = maxAmmo;
        } else
        {
            remainingAmmo += addAmmo;
        }
    }
    public void EatFoodPlayer()
    {
        if (itemFood>0 && UnityEngine.Input.GetKeyDown(KeyCode.Q) && hunger<maxHunger)
        {
            itemFood--;
            GiveFoodPlayer(25);
        }
    }
    public void UseMedKitPlayer()
    {
        if (itemMedkit>0 && UnityEngine.Input.GetKeyDown(KeyCode.F) && health<maxHealth)
        {
            itemMedkit--;
            HealPlayer(25);
        }
    }
    private void HealPlayer(int healAmount)
    {
        if (health+healAmount > maxHealth)
        {
            health = maxHealth;
        } else
        {
            health += healAmount;
        }
    }
    private void GiveFoodPlayer(int foodAmount)
    {
        if (hunger + foodAmount > maxHunger)
        {
            hunger = maxHunger;
        }
        else
        {
            hunger += foodAmount;
        }
    }
}

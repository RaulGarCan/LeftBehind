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
    public float playerJumpforce;
    public int playerSpeed, maxHealth, maxHunger, maxRadiation, maxAmmo, maxMagAmmo;
    private int health, hunger, radiation, remainingAmmo, magazineAmmo, playerSpeedOrig, hungerMultiplier, itemMedkit, itemFood, scorePlayer, sprintSpeedPlayer;
    private bool isVulnerable, isReloading, isHungry, isRadiating;
    private float lastTimeShoot, lastTimeHunger, lastTimeRad;
    private string ammoString;
    public bool isTutorial;
    private bool enableInfiniteAmmo;
    private DifficultyControl difficultyControl;
    private void Start()
    {
        difficultyControl = GetComponent<DifficultyControl>();
        LoadDifficultySettings();

        Time.timeScale = 1f;
        rigPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
        animPlayer = GetComponent<Animator>();
        health = maxHealth;
        hunger = maxHunger;
        radiation = maxRadiation;
        remainingAmmo = maxAmmo;
        magazineAmmo = 0;
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
        sprintSpeedPlayer = 0;

        if (isTutorial)
        {
            LoadTutorialStats();
        }

        RefillGun();

        //LoadPlayerPersistStats();

        UpdateHUDInfo();
    }
    private void Update()
    {
        if (!isTutorial) 
        {
            JumpPlayer();
        } else
        {
            if (magazineAmmo == 0 && enableInfiniteAmmo) 
            {
                magazineAmmo++;
            }
        }
        ShootPlayer();
        ReloadGunPlayer();
        FlipSpritePlayer();
        animPlayer.SetFloat("VelocityAbsX", Math.Abs(rigPlayer.velocity.x));
        animPlayer.SetFloat("VelocityY", rigPlayer.velocity.y);
        UpdateHUDInfo();
        if (radiation <= 0 && Time.time > lastTimeRad + 3f)
        {
            lastTimeRad = Time.time;
            ReduceHealthPlayer();
        }
        if (hunger <= 0 && Time.time > lastTimeHunger + 3f)
        {
            lastTimeHunger = Time.time;
            ReduceHealthPlayer();
        }
        if (isHungry && !isTutorial)
        {
            isHungry = false;
            Invoke("ReduceHungerPlayer", 4f);
        }
        EatFoodPlayer();
        UseMedKitPlayer();
        animPlayer.SetBool("isGrounded", isGrounded());
    }
    private void LoadPlayerPersistStats()
    {

    }
    private void LoadDifficultySettings()
    {
        maxHealth = (int)(maxHealth*difficultyControl.GetPlayerHealthMultiplier());
    }
    private void FixedUpdate()
    {
        if (!isTutorial)
        {
            SprintPlayer();
        }
        MovementPlayer();
    }
    private void MovementPlayer()
    {
        float inputX = UnityEngine.Input.GetAxis("Horizontal");

        rigPlayer.velocity = new Vector2(inputX * (playerSpeed+sprintSpeedPlayer), rigPlayer.velocity.y);
    }
    private void JumpPlayer()
    {
        if (TouchFloor() && !isReloading && UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            rigPlayer.AddForce(Vector2.up * playerJumpforce, ForceMode2D.Impulse);
        }
    }
    private void SprintPlayer()
    {
        if (UnityEngine.Input.GetKey(KeyCode.LeftShift) && !isReloading)
        {
            sprintSpeedPlayer = 2;
        } else
        {
            sprintSpeedPlayer = 0;
        }
    }
    private void FlipSpritePlayer()
    {
        if (rigPlayer.velocity.x < -0.01f)
        {
            spritePlayer.flipX = true;
        }
        else if (rigPlayer.velocity.x > 0.01f)
        {
            spritePlayer.flipX = false;
        }
    }
    private bool TouchFloor()
    {
        RaycastHit2D touch = Physics2D.Raycast(transform.position + new Vector3(0, -1.6f, 0), Vector2.down, 0.45f);

        if (touch.collider != null && touch.collider.CompareTag("Ground"))
        {
            return true;
        }
        return false;
    }

    private void LoadTutorialStats()
    {
        health -= 30;
        hunger -= 25;
        radiation -= 15;
        remainingAmmo = 0;
    }
    private bool isGrounded()
    {
        return TouchFloor();
    }
    public void HurtPlayer(int damage, int rad)
    {
        if (isVulnerable)
        {
            ReduceRadiationPlayer(rad);
            Debug.Log("PlayerHealth: "+health);
            health-=damage;
            MakeInvunerable();
            Invoke("MakeVulnerable", 1.5f);
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
            // Force FrontShooting
            if (GetComponent<SpriteRenderer>().flipX && transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x < 0)
            {
                return;
            } else if (!GetComponent<SpriteRenderer>().flipX && transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 0)
            {
                return;
            }
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

        enableInfiniteAmmo = true;
    }
    private void UpdateHUDInfo()
    {
        hudControl.SetAmmoHUD(ammoString);
        hudControl.SetHealthHUD(health, maxHealth);
        hudControl.SetHungerHUD(hunger, maxHunger);
        hudControl.SetRemAmmoHUD(remainingAmmo);
        hudControl.SetRadHUD(radiation, maxRadiation);
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
    }
    private void ReduceHealthPlayer()
    {
        health--;
        if (health<=0)
        {
            DeathPlayer();
        }
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
            HealPlayer(30);
            HealRadPlayer(15);
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
    private void HealRadPlayer(int radHealAmount)
    {
        if (radiation + radHealAmount > maxRadiation)
        {
            radiation = maxRadiation;
        }
        else
        {
            radiation += radHealAmount;
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

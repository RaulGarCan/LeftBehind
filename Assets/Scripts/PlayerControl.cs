using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    public GameObject deathMenuCanvas, bullet, HUD, thoughts;
    private HUDControl hudControl;
    private MenuControl menuControl;
    public float playerJumpforce;
    public int playerSpeed, maxHealth, maxHunger, maxRadiation, maxAmmo, maxMagAmmo;
    private int health, hunger, radiation, remainingAmmo, magazineAmmo, playerSpeedOrig, hungerMultiplier, itemMedkit, itemFood, scorePlayer, sprintSpeedPlayer, currentLevel;
    private bool isVulnerable, isReloading, isHungry, isRadiating, isThinking, isDead, isPaused, isEating, isHealing;
    private float lastTimeShoot, lastTimeHunger, lastTimeRad, startTimeScene, lastTimeGrounded;
    private string ammoString;
    public bool isTutorial;
    private DifficultyControl difficultyControl;
    public Vector3 lastGroundPoint;
    private bool canMove;
    private void Start()
    {
        startTimeScene = Time.time;

        currentLevel = 1;

        isDead = false;
        isPaused = false;

        difficultyControl = GetComponent<DifficultyControl>();
        LoadDifficultySettings();

        Time.timeScale = 1f;
        canMove = true;
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
        isEating = false;
        isHealing = false;
        playerSpeedOrig = playerSpeed;
        ammoString = magazineAmmo.ToString();
        hungerMultiplier = 5;
        isHungry = true;
        isThinking = false;
        itemMedkit = 0;
        itemFood = 0;
        sprintSpeedPlayer = 0;
        menuControl = GetComponent<MenuControl>();

        if (isTutorial)
        {
            LoadTutorialStats();
        }
        else if(currentLevel>1)
        {
            LoadPlayerPersistStats();
        }

        RefillGun();
        UpdateHUDInfo();
    }
    private void Update()
    {
        if (Time.timeScale>0f)
        {
            isPaused = false;
        }
        PauseGame();
        if (!isTutorial) 
        {
            JumpPlayer();
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
    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deathMenuCanvas.activeSelf)
        {
            isPaused = true;
            hudControl.PauseMenu();
        }
    }
    private void ChangeThoughtsPlayer(string newThought)
    {
        if (isThinking)
        {
            return;
        }
        isThinking = true;
        thoughts.SetActive(true);
        StartCoroutine(LoadThoughtsText(newThought, thoughts.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>()));
        if (!newThought.ToLower().Contains("e to interact")) {
            Invoke("FadeThinking", 2f);
        }
    }
    private void FadeThinking()
    {
        thoughts.transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("FadeOut");
        thoughts.transform.GetChild(0).GetChild(1).GetComponent<Animator>().SetTrigger("FadeOut");
        Invoke("StopThinking", 2f);
    }
    private void StopThinking()
    {
        thoughts.SetActive(false);
        isThinking = false;
    }
    IEnumerator LoadThoughtsText(string thought, TMP_Text text)
    {
        text.text = "";
        for (int i=0; i<thought.Length; i++)
        {
            text.text += thought.ToCharArray()[i];
            yield return new WaitForSeconds(0.02f);
        }
    }
    private void LoadPlayerPersistStats()
    {
        health = PlayerPrefs.GetInt("health");
        hunger = PlayerPrefs.GetInt("hunger");
        radiation = PlayerPrefs.GetInt("radiation");
        remainingAmmo = PlayerPrefs.GetInt("remainingAmmo");
        magazineAmmo = PlayerPrefs.GetInt("magazineAmmo");
        itemMedkit = PlayerPrefs.GetInt("itemMedkit");
        itemFood = PlayerPrefs.GetInt("itemFood");
        scorePlayer = PlayerPrefs.GetInt("scorePlayer");
    }
    private void SavePlayerPersistStats()
    { 
        PlayerPrefs.SetInt("health", health);
        PlayerPrefs.SetInt("hunger", hunger);
        PlayerPrefs.SetInt("radiation", radiation);
        PlayerPrefs.SetInt("remainingAmmo", remainingAmmo);
        PlayerPrefs.SetInt("magazineAmmo", magazineAmmo);
        PlayerPrefs.SetInt("itemMedkit", itemMedkit);
        PlayerPrefs.SetInt("itemFood", itemFood);
        PlayerPrefs.SetInt("scorePlayer", scorePlayer);
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
    public void ExitTutorial()
    {
        if (health==maxHealth)
        {
            if (hunger == maxHunger)
            {
                
                if (remainingAmmo <= 0)
                {
                    ChangeThoughtsPlayer("Hold E to interact");
                    if (Input.GetKey(KeyCode.E))
                    {
                        menuControl.FadeScene("Level1");
                    }
                } else
                {
                    ChangeThoughtsPlayer("My gun is empty...");
                }
            } else
            {
                ChangeThoughtsPlayer("I should eat...");
            }
        } else
        {
            ChangeThoughtsPlayer("I'm hurt...");
        }
    }
    private void MovementPlayer()
    {
        if (isPaused || isDead || !canMove)
        {
            rigPlayer.velocity = Vector2.zero;
            return;
        }
        float inputX = UnityEngine.Input.GetAxis("Horizontal");

        rigPlayer.velocity = new Vector2(inputX * (playerSpeed+sprintSpeedPlayer), rigPlayer.velocity.y);
    }
    private void JumpPlayer()
    {
        if (isPaused || isDead || !canMove)
        {
            return;
        }
        if (TouchFloor(false) && !isReloading && UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            rigPlayer.AddForce(Vector2.up * playerJumpforce, ForceMode2D.Impulse);
        }
    }
    private void SprintPlayer()
    {
        if (isPaused || isDead)
        {
            return;
        }
        if (UnityEngine.Input.GetKey(KeyCode.LeftShift) && !isReloading && !isEating && !isHealing)
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
    private bool TouchFloor(bool ignoreCoyoteTime)
    {
        if (Time.time <= lastTimeGrounded + 0.3f && !ignoreCoyoteTime) // Coyote Time
        {
            return true;
        }

        RaycastHit2D touchCenter = Physics2D.Raycast(transform.position + new Vector3(0, -1.1f, 0), Vector2.down, 0.98f);
        RaycastHit2D touchLeft = Physics2D.Raycast(transform.position + new Vector3(-0.3f, -1.1f, 0), Vector2.down, 0.98f);
        RaycastHit2D touchRight = Physics2D.Raycast(transform.position + new Vector3(0.3f, -1.1f, 0), Vector2.down, 0.98f);

        if ((touchCenter.collider != null && touchCenter.collider.CompareTag("Ground")) || (touchLeft.collider != null && touchLeft.collider.CompareTag("Ground")) || (touchRight.collider != null && touchRight.collider.CompareTag("Ground")))
        {
            lastTimeGrounded = Time.time;
            return true;
        }
        return false;
    }

    private void LoadTutorialStats()
    {
        health -= 30;
        hunger -= 25;
        //radiation -= 15;
        remainingAmmo = 0;
    }
    private bool isGrounded()
    {
        return TouchFloor(false);
    }
    public void HurtPlayer(int damage, int rad)
    {
        if (isVulnerable && !isDead)
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
        isDead = true;
        rigPlayer.velocity = Vector3.zero;
        deathMenuCanvas.SetActive(true);
        Invoke("Pause",0.001f);
    }
    private void SlowGame()
    {
        Time.timeScale = 0.2f;
    }
    public void HurtFallPlayer()
    {
        if (isDead)
        {
            return;
        }
        health--;
        spritePlayer.color = Color.red;
        if (health <= 0)
        {
            DeathPlayer();
        }
        else
        {
            //transform.position = startPos;
            //transform.position = lastPos;
            transform.position = new Vector3((int)lastGroundPoint.x,lastGroundPoint.y,0);
            FreezePlayer();
            Invoke("UnfreezePlayer", 0.2f);
        }
        Invoke("MakeVulnerable", 1.5f);
    }
    private void Pause()
    {
        Time.timeScale = 0f;
    }
    private void FreezePlayer()
    {
        canMove = false;
    }
    private void UnfreezePlayer()
    {
        canMove = true;
    }
    private void ResumeGame()
    {
        Time.timeScale = 1;
    }
    private void ShootPlayer()
    {
        if (isDead || isPaused)
        {
            return;
        }
        if (TouchFloor(false) && UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) && magazineAmmo>0 && Time.time > lastTimeShoot+0.3f && !isReloading)
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
        if (isPaused || isDead)
        {
            return;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && magazineAmmo<maxMagAmmo && !isReloading && remainingAmmo>0 && TouchFloor(false))
        {
            isReloading = true;
            if (playerSpeed > playerSpeedOrig/2)
            {
                playerSpeed /= 2;
            }
            ammoString = "??";
            Invoke("RefillGun",2.5f);
        }
    }
    public void RefillGun(){
        int neededAmmo = maxMagAmmo-magazineAmmo;
        isReloading = false;
        if (!isHealing && !isEating)
        {
            playerSpeed = playerSpeedOrig;
        }

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
        hudControl.SetHealthHUD(health, maxHealth);
        hudControl.SetHungerHUD(hunger, maxHunger);
        hudControl.SetRemAmmoHUD(remainingAmmo);
        hudControl.SetRadHUD(radiation, maxRadiation);
        hudControl.SetTimerHUD((int)(Time.time-startTimeScene)/60, (int)(Time.time-startTimeScene)%60);
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
        if (isEating)
        {
            return;
        }
        if (isPaused || isDead)
        {
            return;
        }
        if (itemFood>0 && UnityEngine.Input.GetKeyDown(KeyCode.Q) && hunger<maxHunger && TouchFloor(false))
        {
            itemFood--;
            isEating = true;
            if (playerSpeed > playerSpeedOrig / 2)
            {
                playerSpeed /= 2;
            }
            Invoke("StartEating", 1.5f);
        }
    }
    private void StartEating()
    {
        GiveFoodPlayer(25);
        if (!isReloading && !isHealing)
        {
            playerSpeed = playerSpeedOrig;
        }
        isEating = false;
    }
    public void UseMedKitPlayer()
    {
        if (isHealing)
        {
            return;
        }
        if (isPaused || isDead)
        {
            return;
        }
        if (itemMedkit>0 && UnityEngine.Input.GetKeyDown(KeyCode.F) && health<maxHealth && TouchFloor(false))
        {
            itemMedkit--;
            isHealing = true;
            if (playerSpeed > playerSpeedOrig / 2)
            {
                playerSpeed /= 2;
            }
            Invoke("StartHealing", 1.5f);
            //HealRadPlayer(15);
        }
    }

    private void StartHealing()
    {
        HealPlayer(30);
        if (!isEating && !isReloading)
        {
            playerSpeed = playerSpeedOrig;
        }
        isHealing = false;
    }
    public void NextLevel()
    {
        Debug.Log("CurrrentLevel: " + currentLevel);
        SavePlayerPersistStats();
        SceneManager.LoadScene("Level" + currentLevel);
        currentLevel++;
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

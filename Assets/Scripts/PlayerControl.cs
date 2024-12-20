using System;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    public GameObject deathMenuCanvas, bullet, HUD, thoughts, soundManager, backgroundMusic;
    private HUDControl hudControl;
    private MenuControl menuControl;
    private AudioSource audioSource, footstepsAudioSource, landAudioSource;
    private SoundControl soundControl;
    public float playerJumpforce, maxStamina, playerSpeed;
    public int maxHealth, maxHunger, maxRadiation, maxAmmo, maxMagAmmo;
    private int health, hunger, radiation, remainingAmmo, magazineAmmo, hungerMultiplier, itemMedkit, itemFood, scorePlayer, sprintSpeedPlayer, currentLevel, fractureSpeedPenalty;
    private bool isVulnerable, isReloading, isHungry, isRadiating, isThinking, isDead, isPaused, isEating, isHealing, canMove, canRun, isFallingFast, isBusy, isFractured, isBleeding;
    private float lastTimeShoot, lastTimeHunger, lastTimeRad, startTimeScene, lastTimeGrounded, stamina, lastBleedTime, playerSpeedOrig;
    private string ammoString;
    public bool isTutorial;
    private DifficultyControl difficultyControl;
    public Vector3 lastGroundPoint;
    
    [Header("Final Game Stats")]
    public int itemsCollected, ammoUsed, enemiesKilled, hitCount;
    public float totalTime;
    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("MenuMusic") != null)
        {
            GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<MenuMusicPersist>().StopMusic();
        }

        audioSource = GetComponents<AudioSource>()[0];
        footstepsAudioSource = GetComponents<AudioSource>()[1];
        landAudioSource = GetComponents<AudioSource>()[2];
        soundControl = soundManager.GetComponent<SoundControl>();

        startTimeScene = Time.time;

        isDead = false;
        isPaused = false;

        difficultyControl = GetComponent<DifficultyControl>();
        LoadDifficultySettings();

        itemsCollected = 0;
        totalTime = 0f;
        ammoUsed = 0;
        enemiesKilled = 0;
        hitCount = 0;

        Time.timeScale = 1f;
        canMove = true;
        canRun = true;
        isFallingFast = false;
        rigPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
        animPlayer = GetComponent<Animator>();
        health = maxHealth;
        hunger = maxHunger;
        radiation = maxRadiation;
        remainingAmmo = maxAmmo;
        stamina = maxStamina;
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
        isBusy = false;
        isFractured = false;
        isBleeding = false;
        itemMedkit = 0;
        itemFood = 0;
        sprintSpeedPlayer = 0;
        fractureSpeedPenalty = 1;
        menuControl = GetComponent<MenuControl>();

        UpdateCurrentLevel();

        Debug.Log(isTutorial);
        Debug.Log(currentLevel);

        if (isTutorial)
        {
            LoadTutorialStats();
        }
        else if (currentLevel > 1)
        {
            LoadPlayerPersistStats();
        }

        RefillGun();
        StartCoroutine(PlayFootstepsSound());
        StartCoroutine(IsFallingFast());

        UpdateHUDInfo();
    }
    private void Update()
    {
        if (isEating || isReloading || isHealing)
        {
            isBusy = true;
        } else
        {
            isBusy = false;
        }
        if (Time.timeScale > 0f)
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

        FractureEffect();
        BleedEffect();

        UpdateHUDInfo();
    }
    private void BleedEffect()
    {
        if (isBleeding && Time.time>lastBleedTime+2f)
        {
            if (!isBleeding)
            {
                StopCoroutine(NaturalBleedHeal());
            }
            StartCoroutine(NaturalBleedHeal());
            ReduceHealthPlayer();
            hudControl.EnableBleed();
            lastBleedTime = Time.time;
        }
    }
    IEnumerator NaturalBleedHeal()
    {
        while (true)
        {
            int rand = UnityEngine.Random.Range(1,101);
            if (rand<=1)
            {
                isBleeding = false;
                hudControl.DisableBleed();
            }
            yield return new WaitForSeconds(10f);
        }
    }
    public void BleedPlayer(int chancePercent)
    {
        if (chancePercent == 0)
        {
            return;
        }
        int rand = UnityEngine.Random.Range(1,101);
        if (rand <= chancePercent)
        {
            isBleeding = true;
        }
    }
    private void FractureEffect()
    {
        if (isFractured)
        {
            fractureSpeedPenalty = 2;
            canRun = false;
            hudControl.EnableFracture();
        }
    }
    public void FracturePlayer(int chancePercent)
    {
        if (chancePercent==0)
        {
            return;
        }
        int rand = UnityEngine.Random.Range(1, 101);
        if (rand <= chancePercent)
        {
            isFractured = true;
        }
    }
    private void DeathMenuMusic(){
        backgroundMusic.GetComponent<AudioSource>().pitch = 0.5f;
        backgroundMusic.GetComponent<AudioSource>().volume = 1f;
    }
    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deathMenuCanvas.activeSelf)
        {
            isPaused = true;
            hudControl.PauseMenu();
        }
    }
    IEnumerator IsFallingFast()
    {
        while (true)
        {
            if (landAudioSource.isPlaying)
            {
                landAudioSource.Stop();
            }
            if (rigPlayer.velocity.y < -12)
            {
                isFallingFast = true;
            }
            if (isFallingFast && isGrounded())
            {
                Debug.Log("LandSound");
                PlayLandSound();
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private void PlayLandSound()
    {
        soundControl.PlayLandSound(audioSource);
        isFallingFast = false;
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
    private void UpdateCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel",1);
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

        int isBleedingInt = PlayerPrefs.GetInt("isBleeding");
        int isFracturedInt = PlayerPrefs.GetInt("isFracture");
        if (isBleedingInt == 1)
        {
            isBleeding = true;
        }
        if (isFracturedInt == 1)
        {
            isFractured = true;
        }
        
        itemsCollected = PlayerPrefs.GetInt("itemsCollected");
        ammoUsed = PlayerPrefs.GetInt("ammoUsed");
        enemiesKilled = PlayerPrefs.GetInt("enemiesKilled");
        hitCount = PlayerPrefs.GetInt("hitCount");
        totalTime = PlayerPrefs.GetFloat("totalTime");
    }
    private void SavePlayerPersistStats()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel);

        totalTime += Time.time - startTimeScene;

        PlayerPrefs.SetInt("health", health);
        PlayerPrefs.SetInt("hunger", hunger);
        PlayerPrefs.SetInt("radiation", radiation);
        PlayerPrefs.SetInt("remainingAmmo", remainingAmmo);
        PlayerPrefs.SetInt("magazineAmmo", magazineAmmo);
        PlayerPrefs.SetInt("itemMedkit", itemMedkit);
        PlayerPrefs.SetInt("itemFood", itemFood);
        PlayerPrefs.SetInt("scorePlayer", scorePlayer);

        int isBleedingInt = 0;
        int isFracturedInt = 0;
        if (isBleeding)
        {
            isBleedingInt = 1;
        } 
        if (isFractured) 
        {
            isFracturedInt = 1;
        }
        PlayerPrefs.SetInt("isBleeding", isBleedingInt);
        PlayerPrefs.SetInt("isFracture", isFracturedInt);


        PlayerPrefs.SetInt("itemsCollected", itemsCollected);
        PlayerPrefs.SetInt("ammoUsed",ammoUsed);
        PlayerPrefs.SetInt("enemiesKilled", enemiesKilled);
        PlayerPrefs.SetInt("hitCount", hitCount);
        PlayerPrefs.SetFloat("totalTime", totalTime);

    }
    private void LoadDifficultySettings()
    {
        maxHealth = (int)(maxHealth*difficultyControl.GetPlayerHealthMultiplier());
    }
    private void FixedUpdate()
    {
        //Debug.Log("Stamina "+stamina);
        if (!isTutorial)
        {
            SprintPlayer();
            if (sprintSpeedPlayer == 0 && stamina<maxStamina)
            {
                IncreaseStamina();
            }
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

        rigPlayer.velocity = new Vector2(inputX * ((playerSpeed/fractureSpeedPenalty) + sprintSpeedPlayer), rigPlayer.velocity.y);
    }
    IEnumerator PlayFootstepsSound()
    {
        while (true)
        {
            Debug.Log("Looking for Steps...");
            if (Math.Abs(rigPlayer.velocity.x) > 0.1f)
            {
                soundControl.PlayFootstepsSound(footstepsAudioSource);
                yield return new WaitForSeconds(1f);
            } else
            {
                footstepsAudioSource.Stop();
                yield return new WaitForEndOfFrame();
            }
        }
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
            soundControl.PlayJumpSound(audioSource);
        }
    }
    private void SprintPlayer()
    {
        if (isPaused || isDead)
        {
            return;
        }
        if (UnityEngine.Input.GetKey(KeyCode.LeftShift) && !isReloading && !isEating && !isHealing && canRun && Math.Abs(rigPlayer.velocity.x) > 0.01f)
        {
            sprintSpeedPlayer = 2;
            ReduceStamina();
        } else
        {
            sprintSpeedPlayer = 0;
        }
    }
    private void ReduceStamina()
    {
        if (stamina > 0)
        {
            stamina -= 2f;
        } else if (stamina <= 0)
        {
            canRun = false;
            playerSpeed *= 0.7f;
            Invoke("RemoveSprintPenalty",1f);
        }
    }
    private void RemoveSprintPenalty()
    {
        playerSpeed = playerSpeedOrig;
    }

    private void IncreaseStamina()
    {
        if (stamina+1>maxStamina)
        {
            stamina = maxStamina;
        } else
        {
            stamina += 1f;
        }
        if (stamina>=maxStamina/2)
        {
            canRun = true;
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
        return TouchFloor(true);
    }
    public void HurtPlayer(int damage, int rad, int bleedChancePercent, int fractureChancePercent)
    {
        if (isVulnerable && !isDead)
        {
            hitCount++;
            ReduceRadiationPlayer(rad);
            Debug.Log("PlayerHealth: "+health);
            health-=damage;
            BleedPlayer(bleedChancePercent);
            FracturePlayer(fractureChancePercent);
            MakeInvunerable();
            Invoke("MakeVulnerable", 1.5f);
            if (health<=0)
            {
                audioSource.GetComponent<AudioSource>().pitch = 0.5f;
                audioSource.GetComponent<AudioSource>().volume = 1f;
                soundControl.PlayPlayerHurtSound(audioSource);
                DeathPlayer();
            } else
            {
                soundControl.PlayPlayerHurtSound(audioSource);
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
        DeathMenuMusic();
        Invoke("Pause",0.001f);
    }
    public void HurtFallPlayer()
    {
        if (isDead)
        {
            return;
        }
        hitCount++;
        health-=20;
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
            if (GetComponent<SpriteRenderer>().flipX && transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x < 1f)
            {
                return;
            } else if (!GetComponent<SpriteRenderer>().flipX && transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x > -1f)
            {
                return;
            }
            Instantiate(bullet, transform);
            ammoUsed++;
            magazineAmmo--;
            lastTimeShoot = Time.time;
            ammoString = magazineAmmo.ToString();
            soundControl.PlayShootSound(audioSource);

            GunRecoil();
        }
    }
    private void GunRecoil()
    {
        if (rigPlayer.velocity.x>0.01f)
        {
            return;
        }
        if (!spritePlayer.flipX)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 0.1f, transform.position.y, 0f), Time.time * playerSpeed);
        } else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 0.1f, transform.position.y, 0f), Time.time * playerSpeed);
        }
    }
    private void ReloadGunPlayer()
    {
        if (isPaused || isDead || isReloading || isBusy)
        {
            return;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && magazineAmmo<maxMagAmmo && remainingAmmo>0 && TouchFloor(false))
        {
            isReloading = true;
            if (playerSpeed > playerSpeedOrig/2)
            {
                playerSpeed /= 2;
            }
            ammoString = "??";
            soundControl.PlayReloadSound(audioSource);
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
        hudControl.SetStaminaHUD(stamina, maxStamina);
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
        itemsCollected++;
        scorePlayer += addScore;
    }
    public void PickupMedKit(int addScore)
    {
        itemMedkit++;
        itemsCollected++;
        scorePlayer += addScore;
    }
    public void PickupAmmo(int addAmmo)
    {
        itemsCollected++;
        scorePlayer += addAmmo;
        if (remainingAmmo==maxAmmo)
        {
            scorePlayer += addAmmo/2;
        }
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
        if (isEating || isBusy)
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
            soundControl.PlayEatSound(audioSource);
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
        if (isHealing || isBusy)
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
            soundControl.PlayHealSound(audioSource);
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
    public void AddHitCount()
    {
        hitCount++;
    }
    public void AddEnemiesKilled()
    {
        enemiesKilled++;
    }
    public void NextLevel()
    {
        currentLevel++;
        Debug.Log("CurrrentLevel: " + currentLevel);
        SavePlayerPersistStats();
        SceneManager.LoadScene("Level" + currentLevel);
    }
    public void FinalLevel()
    {
        SavePlayerPersistStats();
        SceneManager.LoadScene("LevelFinal");
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
        isFractured = false;
        isBleeding = false;
        canRun = true;
        hudControl.DisableBleed();
        hudControl.DisableFracture();
        fractureSpeedPenalty = 1;
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

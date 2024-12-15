using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlobControl : MonoBehaviour
{
    private bool movingToEndPos;
    private Vector3 startPos, endPos;
    public int finalPosX, damage, radAmount, maxHealth;
    public float speed;
    public GameObject blobHealthBar, foodItem, ammoItem, medkitItem;
    private Image imageHealthBar;
    private int health;
    private DifficultyControl difficultyControl;
    public bool isChasing;
    private void Start()
    {
        difficultyControl = GetComponent<DifficultyControl>();
        LoadDifficultySettings();
        imageHealthBar = blobHealthBar.transform.GetChild(1).GetComponent<Image>();
        startPos = transform.position;
        movingToEndPos = true;
        endPos = new Vector3(finalPosX, transform.position.y, transform.position.z);
        health = maxHealth;
        blobHealthBar.SetActive(false);
        isChasing = false;
    }
    private void DropLoot()
    {
        int rand = Random.Range(0, 4);
        if (rand == 1)
        {
            SpawnLootItem();
        }
    }
    private void SpawnLootItem()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                Instantiate(ammoItem).transform.position = transform.position;
                break; 
            case 1:
                Instantiate(medkitItem).transform.position = transform.position;
                break;
            case 2:
                Instantiate(foodItem).transform.position = transform.position;
                break;
        }
    }
    private void LoadDifficultySettings()
    {
        Debug.Log(difficultyControl.GetEnemyDamageMultiplier());
        maxHealth = (int)(maxHealth * difficultyControl.GetEnemyHealthMultiplier());
        damage = (int)(damage * difficultyControl.GetEnemyDamageMultiplier());
        speed = speed * difficultyControl.GetEnemySpeedMultiplier();
    }
    private void Update()
    {
        if (!isChasing)
        {
            if (movingToEndPos)
            {
                MoveToPos(transform.position, endPos);
            }
            else
            {
                MoveToPos(transform.position, startPos);
            }
        }
        if (health<=0)
        {
            DropLoot();
            Destroy(gameObject);
        }
        UpdateHealthBarBlob();
    }
    private void MoveToPos(Vector3 startPos, Vector3 finalPos)
    {
        transform.position = Vector3.MoveTowards(startPos, finalPos, speed * Time.deltaTime);
        if (transform.position == finalPos)
        {
            movingToEndPos = !movingToEndPos;
        }
    }
    private void UpdateHealthBarBlob()
    {
        if (!blobHealthBar.activeSelf && health<maxHealth)
        {
            blobHealthBar.SetActive(true);
        }
        imageHealthBar.fillAmount = (float)health/maxHealth;
    }
    public void HurtBlob()
    {
        health--;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().HurtPlayer(damage, radAmount, 30, 0);
        }
    }
    public int GetHealth()
    {
        return health;
    }
}

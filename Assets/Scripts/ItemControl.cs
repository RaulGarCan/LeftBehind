using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    public enum ItemType
    {
        food,
        medkit,
        ammo
    }
    public ItemType type;
    public int points;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PickupMethod(collision.gameObject);
            //Destroy(gameObject);
        }
    }
    private void PickupMethod(GameObject gameObject)
    {
        switch (type)
        {
            case ItemType.food:
                gameObject.GetComponent<PlayerControl>().PickupFoodCan(points);
                break;
            case ItemType.medkit:
                gameObject.GetComponent<PlayerControl>().PickupMedKit(points);
                break;
            case ItemType.ammo:
                gameObject.GetComponent<PlayerControl>().PickupAmmo(points);
                break;
        }
    }
}
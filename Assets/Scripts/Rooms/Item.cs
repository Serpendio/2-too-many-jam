using Core;
using Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    [HideInInspector] public int itemID;
    //0: Spell
    //1: Modifier
    //2: Shard(s)
    //3: Health refill
    //4: Max health increase

    [HideInInspector] public IInventoryItem item;
    [HideInInspector] public int cost;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && Locator.CurrencyManager.GoldAmount >= cost)
        {
            Locator.CurrencyManager.AddGold(-cost);
            Creature.Player player = collision.gameObject.GetComponent<Creature.Player>();
            switch (itemID)
            {
                case 0:
                    //Spell
                    player.Inventory.AddToInventory(item);
                    break;

                case 1:
                    //Modifier
                    player.Inventory.AddToInventory(item);
                    break;

                case 2:
                    //Shard(s)
                    //To be added
                    break;

                case 3:
                    //Health refill
                    player.RefillHealth();
                    break;

                case 4:
                    //Max health increase
                    int healthIncrease = Random.Range(5, 20);
                    player.SetMaxHealth(player.maxHealth + healthIncrease, true);
                    break;
            }
            Destroy(this.gameObject);
        }
    }
}

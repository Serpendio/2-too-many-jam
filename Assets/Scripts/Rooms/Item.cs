using Core;
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

    [HideInInspector] public dynamic item;
    [HideInInspector] public int cost;
    private CurrencyManager currencyManager;

    private void Start() {
        Locator.ProvideCurrencyManager(currencyManager);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && currencyManager.GoldAmount >= cost)
        {
            currencyManager.AddGold(-cost);
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

                    break;

                case 3:
                    //Health refill

                    break;

                case 4:
                    //Max health increase

                    break;
            }
            Destroy(this.gameObject);
        }
    }
}

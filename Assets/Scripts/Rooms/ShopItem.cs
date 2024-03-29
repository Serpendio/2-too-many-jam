using Core;
using Inventory;
using UnityEngine;

namespace Rooms
{
    public class ShopItem : MonoBehaviour
    {
        [HideInInspector] public int itemID;
        //0: Spell
        //1: Modifier
        //2: Shard(s)
        //3: Health refill
        //4: Max health increase

        [HideInInspector] public IInventoryItem item;
        [HideInInspector] public int cost;
        [HideInInspector] public int shardAmount; //For shards only

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Player") && Locator.Inventory.Currency.GoldAmount >= cost)
            {
                Locator.Inventory.Currency.AddGold(-cost);
                switch (itemID)
                {
                    case 0:
                        //Spell
                        Locator.Inventory.AddToInventory(item);
                        break;

                    case 1:
                        //Modifier
                        Locator.Inventory.AddToInventory(item);
                        break;

                    case 2:
                        //Shard(s)
                        Core.Locator.Inventory.Currency.AddSpellShards(shardAmount);
                        break;

                    case 3:
                        //Health refill
                        Locator.Player.RefillHealth();
                        break;

                    case 4:
                        //Max health increase
                        int healthIncrease = Random.Range(5, 20);
                        Locator.Player.SetMaxHealth(Locator.Player.maxHealth + healthIncrease, true);
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}

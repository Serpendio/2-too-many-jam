using Core;
using Inventory;
using TMPro;
using UI;
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
        [HideInInspector] public Shop shop;

        public void Setup()
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cost.ToString();
            switch (itemID)
            {
                case 0:
                    //Spell

                case 1:
                    //Modifier
                    transform.GetComponent<InventorySlot>().SetItem(item);
                    break;

                case 2:
                    //Shard(s)
                    transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shardAmount.ToString();
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Player") && Locator.Inventory.Currency.GoldAmount >= cost)
            {
                switch (itemID)
                {
                    case 0:
                        //Spell

                    case 1:
                        //Modifier
                        if (!Locator.Inventory.HasSpaceForItem)
                            return;

                        Locator.Inventory.AddToInventory(item);
                        break;

                    case 2:
                        //Shard(s)
                        Locator.Inventory.Currency.AddSpellShards(shardAmount);
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
                Locator.Inventory.Currency.AddGold(-cost);
                shop.CloseShop();
                Destroy(gameObject);
            }
        }
    }
}

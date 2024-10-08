using System.Collections.Generic;
using System.Linq;
using Core;
using Creature;
using Spells;
using Spells.Modifiers;
using UnityEngine;

namespace Rooms
{
    public class Shop : MonoBehaviour
    {

        public GameObject[] itemIconPrefabs;
        private Vector3[] itemLocations;

        private const int maxPossibleTier = 3; //Max tier that a spell can be. Sets an upper bound on currentMaxTier
        private int currentMaxTier; //Max tier that a spell can currently spawn in the shop as - ranges based on level. Max value for this variable is maxPossibleTier

        private void Awake()
        {
            itemLocations = new[] { new Vector3(4.5f, 8.5f, 0f), new Vector3(9.5f, 8.5f, 0f), new Vector3(14.5f, 8.5f, 0f) };

            //Generate 3 random and unique item-types
            List<int> chosenitemIndices = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                //Get random items until unique one found
                int randType; //0: spell, 1: modifier, 2: shard(s), 3: health refill, 4: total health increase
                do
                {
                    randType = Random.Range(0, 5);
                } while (chosenitemIndices.Contains(randType));
                chosenitemIndices.Add(randType);


                GameObject itemIcon = Instantiate(itemIconPrefabs[randType], transform.GetChild(0));
                switch (randType)
                {
                    //Spell
                    case 0:
                        int spellCost;
                        Spell spell = GenerateRandomSpell(out spellCost);
                        itemIcon.GetComponent<ShopItem>().item = spell;
                        itemIcon.GetComponent<ShopItem>().cost = spellCost;
                        break;

                    //Modifier
                    case 1:
                        SpellModifier modifier = SpellModifier.AllModifiers[Random.Range(0, SpellModifier.AllModifiers.Count)];
                        int modifierCost = (20 + Random.Range(-2, 3)) * (1 + (int)modifier.Tier);
                        itemIcon.GetComponent<ShopItem>().item = modifier;
                        itemIcon.GetComponent<ShopItem>().cost = modifierCost;
                        break;

                    //Shard(s)
                    case 2:
                        int shardAmount = Random.Range(5,16);
                        int shardCost = shardAmount * 10;
                        itemIcon.GetComponent<ShopItem>().cost = shardCost;
                        itemIcon.GetComponent<ShopItem>().shardAmount = shardAmount;
                        break;

                    //Health refill
                    case 3:
                        itemIcon.GetComponent<ShopItem>().cost = 80 * (int)Locator.Player.maxHealth / 50;
                        break;

                    //Max health increase
                    case 4:
                        itemIcon.GetComponent<ShopItem>().cost = 70 * (int)Locator.Player.maxHealth / 50;
                        break;

                }
                itemIcon.GetComponent<ShopItem>().itemID = randType;
                itemIcon.GetComponent<ShopItem>().Setup();
                itemIcon.GetComponent<ShopItem>().shop = this;
                itemIcon.transform.position = itemLocations[i];
            }
        }

        public void CloseShop()
        {
            Destroy(transform.GetChild(0).gameObject);
        }


        private void Start() {
            Core.Locator.LevelManager.OnPlayerLevelUp.AddListener((int level) =>
            {
                //Update max tier every maxLevel / maxPossibleTier levels to ensure even distribution between level ups
                //(e.g. every 10 levels for max level = 30, maxTier = 3)
                if (level % (Core.Locator.LevelManager.getMaxLevel() / maxPossibleTier) == 0) {
                    currentMaxTier += 1;
                }

            });
        }


        private Spell GenerateRandomSpell(out int cost)
        {

            Element randElement = (Element)Random.Range(0, 6);

            Spell spell = new Spell(new SpellStats
            {
                DamageOnHit = Random.Range(5, 25),
                CastCooldown = Random.Range(0.2f, 2f),
                ManaUsage = Random.Range(5, 25),
                ProjectileSpeed = Random.Range(10, 40),
                Range = Random.Range(5, 25),
            }, randElement, Team.Friendly);

            cost = (int)(spell.BaseStats.DamageOnHit +
                (3 - spell.BaseStats.CastCooldown) * 10 +
                30 - spell.BaseStats.ManaUsage +
                spell.BaseStats.ProjectileSpeed / 2 +
                spell.BaseStats.Range);

            int randNumModifiers = Random.Range(0, 3);
            for (int i = 0; i < randNumModifiers; i++)
            {
                //Add unique modifier
                var uniqueModifiers = SpellModifier.AllModifiers
                    .Where(m => !spell.Modifiers.Contains(m) && m.Tier <= (ModifierTier)currentMaxTier)
                    .ToList();
                
                var randomModifier = uniqueModifiers[Random.Range(0, uniqueModifiers.Count)];
                spell.AddModifier(randomModifier);
                cost += (20 + Random.Range(-2, 3)) * (1 + (int)randomModifier.Tier);
            }

            return spell;
        }
    }
}
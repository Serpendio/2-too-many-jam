using Creature;
using Spells;
using Spells.Modifiers;
using System.Collections.Generic;
using System.Linq;
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


                GameObject itemIcon = new GameObject();
                switch (randType)
                {
                    //Spell
                    case 0:
                        int spellCost;
                        Spell spell = GenerateRandomSpell(out spellCost);
                        itemIcon = Instantiate(itemIconPrefabs[0]);
                        itemIcon.GetComponent<Item>().item = spell;
                        itemIcon.GetComponent<Item>().cost = spellCost;
                        break;

                    
                    //----Possibly pointless cases, keeping them here for maintainability----//
                    //Health refill
                    case 1:
                        itemIcon = Instantiate(itemIconPrefabs[3]);
                        break;

                    //Total health increase
                    case 2:
                        itemIcon = Instantiate(itemIconPrefabs[4]);
                        break;

                    //Modifier
                    case 3:
                        int modifierCost = 0;
                        SpellModifier modifier = SpellModifier.AllModifiers[Random.Range(0, SpellModifier.AllModifiers.Count)];
                        itemIcon = Instantiate(itemIconPrefabs[1]);
                        itemIcon.GetComponent<Item>().item = modifier;
                        itemIcon.GetComponent<Item>().cost = modifierCost;
                        break;
                    
                    //Shard(s)
                    case 4:
                        itemIcon = Instantiate(itemIconPrefabs[2]);
                        break;

                }
                itemIcon.GetComponent<Item>().itemID = randType;
                itemIcon.transform.parent = this.transform;
                itemIcon.transform.position = itemLocations[i];
            }
        }


        private void Start() {
            Core.Locator.LevelManager.PlayerLevelUp.AddListener(() =>
            {
                //Update max tier every maxLevel / maxPossibleTier levels to ensure even distribution between level ups
                //(e.g. every 10 levels for max level = 30, maxTier = 3)
                if (Core.Locator.LevelManager.getCurrentLevel() % (Core.Locator.LevelManager.getMaxLevel() / maxPossibleTier) == 0) {
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

            int randNumModifiers = Random.Range(0, 3);
            for (int i = 0; i < randNumModifiers; i++)
            {
                //Add unique modifier
                var uniqueModifiers = SpellModifier.AllModifiers
                    .Where(m => !spell.Modifiers.Contains(m) && m.Tier <= (ModifierTier)currentMaxTier)
                    .ToList();
                
                var randomModifier = uniqueModifiers[Random.Range(0, uniqueModifiers.Count)];
                spell.AddModifier(randomModifier);
            }
            cost = 0; //TEMP
            return spell;
        }
    }
}
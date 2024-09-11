using System.Collections.Generic;
using System.Linq;
using Core;
using Creature;
using Spells;
using Spells.Modifiers;
using UnityEngine;

namespace UI
{
    public class ShardSpellGenerator
    {
        public enum ShardTiers
        {
            Okay,
            Good,
            Epic,
            Wowza
        }

        public static Dictionary<ShardTiers, int> ShardTierPrices = new()
        {
            {ShardTiers.Okay, 10},
            {ShardTiers.Good, 20},
            {ShardTiers.Epic, 30},
            {ShardTiers.Wowza, 40},
        };

        public static void GenerateSpellFromShard(ShardTiers tier)
        {
            Element element = (Element)Random.Range(1, 6);
            Locator.Inventory.Currency.AddSpellShards(-ShardTierPrices[tier]);

            //Generate spell
            // const int chanceToBeElementSpecificModifier = 30;
            const float minSpellCooldownTime = 0.2f;
            const float maxSpellCooldownTime = 10.0f; //Probably wont ever be reached
            const float minSpellManaUsage = 5.0f;
            const float maxSpellManaUsage = 100.0f; //Probably wont ever be reached
            switch (tier)
            {
                case ShardTiers.Okay:
                {
                    //5% chance of having 0 modifiers, 40% chance of having 1 modifier, 50% chance of having 2 modifiers, 5% chance of having 3 modifiers
                    int numModifiers = GetWeightedRandomInteger(5, 0, 40, 1, 50, 2, 5, 3);

                    Spell spell = new Spell(new SpellStats
                    {
                        DamageOnHit = Random.Range(2, 6),
                        CastCooldown = Mathf.Clamp(numModifiers + Random.Range(-numModifiers*0.75f, numModifiers*0.75f), minSpellCooldownTime, maxSpellCooldownTime),
                        ManaUsage = Mathf.Clamp(numModifiers * 10f + Random.Range(-numModifiers * 5f, numModifiers * 5f), minSpellManaUsage, maxSpellManaUsage),
                        ProjectileSpeed = Random.Range(10, 20),
                        Range = Random.Range(5, 15),
                    }, element, Team.Friendly);

                    for (int i = 0; i < numModifiers; i++)
                    {
                        //Add unique modifier(s)
                        //80% chance of being tier 1, 20% of being tier 2
                        // int randModifierTier = GetWeightedRandomInteger(80, 1, 20, 2);
                        List<SpellModifier> uniqueModifiers = SpellModifier.AllModifiers.Where(m => !spell.Modifiers.Contains(m)).ToList();
                        if (uniqueModifiers.Count == 0) continue;
                        spell.Modifiers.Add(uniqueModifiers[Random.Range(0, uniqueModifiers.Count)]);
                    }
                    Locator.Inventory.AddToHotbar(spell);
                    break;
                }


                case ShardTiers.Good:
                {
                    int numModifiers = GetWeightedRandomInteger(5, 1, 40, 2, 50, 3, 5, 4);

                    Spell spell = new Spell(new SpellStats
                    {
                        DamageOnHit = Random.Range(5, 10),
                        CastCooldown = Mathf.Clamp(numModifiers + Random.Range(-numModifiers * 0.75f, numModifiers * 0.75f), minSpellCooldownTime, maxSpellCooldownTime),
                        ManaUsage = Mathf.Clamp(numModifiers * 10f + Random.Range(-numModifiers * 5f, numModifiers * 5f), minSpellManaUsage, maxSpellManaUsage),
                        ProjectileSpeed = Random.Range(20, 35),
                        Range = Random.Range(10, 20),
                    }, element, Team.Friendly);

                    for (int i = 0; i < numModifiers; i++)
                    {
                        // int randModifierTier = GetWeightedRandomInteger(20, 1, 75, 2, 5, 3);
                        List<SpellModifier> uniqueModifiers = SpellModifier.AllModifiers.Where(m => !spell.Modifiers.Contains(m)).ToList();
                        if(uniqueModifiers.Count == 0) continue;
                        spell.Modifiers.Add(uniqueModifiers[Random.Range(0, uniqueModifiers.Count)]);
                    }
                    Locator.Inventory.AddToHotbar(spell);
                    break;
                }


                case ShardTiers.Epic:
                {
                    int numModifiers = GetWeightedRandomInteger(5, 2, 40, 3, 50, 4, 5, 5);

                    Spell spell = new Spell(new SpellStats
                    {
                        DamageOnHit = Random.Range(8, 15),
                        CastCooldown = Mathf.Clamp(numModifiers + Random.Range(-numModifiers * 0.75f, numModifiers * 0.75f), minSpellCooldownTime, maxSpellCooldownTime),
                        ManaUsage = Mathf.Clamp(numModifiers * 10f + Random.Range(-numModifiers * 5f, numModifiers * 5f), minSpellManaUsage, maxSpellManaUsage),
                        ProjectileSpeed = Random.Range(25, 40),
                        Range = Random.Range(10, 20),
                    }, element, Team.Friendly);

                    for (int i = 0; i < numModifiers; i++)
                    {
                        // int randModifierTier = GetWeightedRandomInteger(15, 1, 45, 2, 40, 3);
                        List<SpellModifier> uniqueModifiers = SpellModifier.AllModifiers.Where(m => !spell.Modifiers.Contains(m)).ToList();
                        if (uniqueModifiers.Count == 0) continue;
                        spell.Modifiers.Add(uniqueModifiers[Random.Range(0, uniqueModifiers.Count)]);
                    }
                    Locator.Inventory.AddToHotbar(spell);
                    break;
                }

                case ShardTiers.Wowza:
                {
                    int numModifiers = GetWeightedRandomInteger(5, 3, 40, 4, 50, 5, 5, 6);

                    Spell spell = new Spell(new SpellStats
                    {
                        DamageOnHit = Random.Range(12, 18),
                        CastCooldown = Mathf.Clamp(numModifiers + Random.Range(-numModifiers * 0.75f, numModifiers * 0.75f), minSpellCooldownTime, maxSpellCooldownTime),
                        ManaUsage = Mathf.Clamp(numModifiers * 10f + Random.Range(-numModifiers * 5f, numModifiers * 5f), minSpellManaUsage, maxSpellManaUsage),
                        ProjectileSpeed = Random.Range(35, 50),
                        Range = Random.Range(15, 25),
                    }, element, Team.Friendly);

                    for (int i = 0; i < numModifiers; i++)
                    {
                        // int randModifierTier = GetWeightedRandomInteger(5, 1, 25, 2, 70, 3);
                        List<SpellModifier> uniqueModifiers = SpellModifier.AllModifiers.Where(m => !spell.Modifiers.Contains(m)).ToList();
                        if(uniqueModifiers.Count == 0) continue;
                        spell.Modifiers.Add(uniqueModifiers[Random.Range(0, uniqueModifiers.Count)]);
                    }
                    Locator.Inventory.AddToHotbar(spell);
                    break;
                }
            }
        }

        private static int GetWeightedRandomInteger(int p0, int v0, int p1, int v1, int p2=0, int v2=0, int p3=0, int v3=0) //Percentage chances (p0,p1,p2,p3) to return v0, v1, v2, or v3 respectively
        {
            float percent = Random.value * 100;
            return percent <= p0 ? v0 : (percent <= p0+p1 ? v1 : (percent <= p0+p1+p2 ? v2 : v3));
        }
    }
}




//COMMENTED OUT FROM SWITCH CASES - MODIFIERS DONT YET HAVE ELEMENTS ASSOCIATED WITH THEM - PUT THIS CODE BACK IN WHEN THEY DO

//List<SpellModifier> uniqueElementSpecificModifiers = uniqueModifiers.Where(m => m.elementType == element).ToList();

////30% chance to be element specific modifier
//if (Random.value <= chanceToBeElementSpecificModifier) {
//    spell.Modifers.Add(uniqueElementSpecificModifiers[Random.Range(0, uniqueElementSpecificModifiers.Count)]);
//}
// else {
//  spell.Modifiers.Add(uniqueModifiers[Random.Range(0, uniqueModifiers.Count)]);
//}
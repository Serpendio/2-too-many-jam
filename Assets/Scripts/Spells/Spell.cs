using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Creature;
using Inventory;
using Spells.Modifiers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spells
{
    public enum Element
    {
        None,
        Fire,
        Water,
        Lightning,
        Air,
        Earth
    }

    [Serializable]
    public struct SpellStats
    {
        public float DamageOnHit;
        public float CastCooldown;
        public float ManaUsage;
        public float ProjectileSpeed;
        public float Range;
        public float Spread;

        public static SpellStats operator +(SpellStats a, SpellStats b) =>
            new()
            {
                DamageOnHit = a.DamageOnHit + b.DamageOnHit,
                CastCooldown = a.CastCooldown + b.CastCooldown,
                ManaUsage = a.ManaUsage + b.ManaUsage,
                ProjectileSpeed = a.ProjectileSpeed + b.ProjectileSpeed,
                Range = a.Range + b.Range,
                Spread = a.Spread + b.Spread
            };
        
        public static SpellStats operator -(SpellStats a, SpellStats b) =>
            new()
            {
                DamageOnHit = a.DamageOnHit - b.DamageOnHit,
                CastCooldown = a.CastCooldown - b.CastCooldown,
                ManaUsage = a.ManaUsage - b.ManaUsage,
                ProjectileSpeed = a.ProjectileSpeed - b.ProjectileSpeed,
                Range = a.Range - b.Range,
                Spread = a.Spread - b.Spread
            };

        public static SpellStats operator *(SpellStats a, SpellStats b) =>
            new()
            {
                DamageOnHit = a.DamageOnHit * b.DamageOnHit,
                CastCooldown = a.CastCooldown * b.CastCooldown,
                ManaUsage = a.ManaUsage * b.ManaUsage,
                ProjectileSpeed = a.ProjectileSpeed * b.ProjectileSpeed,
                Range = a.Range * b.Range,
                Spread = a.Spread * b.Spread
            };
    }

    [Serializable]
    public class Spell : IInventoryItem
    {
        public static IDictionary<Element, Sprite> ElementSprites = new Dictionary<Element, Sprite>
        {
            { Element.None, Resources.Load<Sprite>("Sprites/elementNeutral") },
            { Element.Fire, Resources.Load<Sprite>("Sprites/elementFIRE") },
            { Element.Water, Resources.Load<Sprite>("Sprites/elementWater") },
            { Element.Air, Resources.Load<Sprite>("Sprites/elementAIR") },
            { Element.Lightning, Resources.Load<Sprite>("Sprites/elementElectricty") },
            { Element.Earth, Resources.Load<Sprite>("Sprites/elementearth") }
        };
        
        public static IDictionary<Element, string[]> ElementTerms = new Dictionary<Element, string[]>
        {
            { Element.None, new[] { "Neutrality", "Apathy", "Unfeeling" } },
            { Element.Fire, new[] { "Flame", "Fire", "Heat", "Burning" } },
            { Element.Water, new[] { "Water", "Aqua", "Aquatics", "the Seas" } },
            { Element.Air, new[] { "Wind", "Air", "the Clouds" } },
            { Element.Lightning, new[] { "Lightning", "Electricity", "Sparks", "the Storm" } },
            { Element.Earth, new[] { "Earth", "Grounding", "Rock" } }
        };
        
        public static string[] GenericNouns = { "spell", "hex", "bolt", "missile", "curse", "shot", "charm", "chant" };

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Sprite Icon => ElementSprites[Element];

        [field: SerializeField] public int GridIndex { get; set; } = -1;

        public bool IsOnHotbar;

        public SpellStats BaseStats;
        public Team Team;

        public SpellStats ComputedStats =>
            Modifiers.Aggregate(BaseStats, (spellStats, modifier) => modifier.ModifyStats(spellStats));

        public List<SpellModifier> Modifiers = new();
        public Element Element;

        public float LastCastTime;

        public bool CooldownOver => Time.time - LastCastTime > ComputedStats.CastCooldown;

        public Spell(SpellStats baseStats, Element element, Team team, List<SpellModifier> modifiers = null)
        {
            BaseStats = baseStats;
            Element = element;
            Team = team;
            Modifiers = modifiers ?? new List<SpellModifier>();

            LastCastTime = -ComputedStats.CastCooldown;
            
            Name = GetComputedName();
            Description = "A spell attuned to " + Element;
        }

        public void AddModifier(SpellModifier modifier)
        {
            Modifiers.Add(modifier);
        }

        public void RemoveModifier(SpellModifier modifier)
        {
            Modifiers.Remove(modifier);
        }

        private string GetComputedName()
        {
            var name = "";
            
            var verbModifier = Modifiers
                .Where(m => m.SetDescriptive)
                .OrderBy(_ => Random.value)
                .FirstOrDefault();
            
            var nounModifier = Modifiers
                .Where(m => m.SetNoun && m != verbModifier)
                .OrderBy(_ => Random.value)
                .FirstOrDefault();
            
            var ti = new CultureInfo("en-US", false).TextInfo;
            
            if (verbModifier != null) name += $"{ti.ToTitleCase(verbModifier.Verb)} ";
            if (nounModifier != null) name += ti.ToTitleCase(nounModifier.Noun);
            else name += ti.ToTitleCase(GenericNouns[Random.Range(0, GenericNouns.Length)]);

            name += " of " + ti.ToTitleCase(ElementTerms[Element][Random.Range(0, ElementTerms[Element].Length)]);
            
            return name;
        }
        
        public Spell CombinedWith(Spell other)
        {
            var combinedSpell = new Spell(BaseStats + other.BaseStats, Element, Team, Modifiers.Concat(other.Modifiers).Distinct().OrderBy(_ => Random.value).ToList());
            return combinedSpell;
        }
    }
}
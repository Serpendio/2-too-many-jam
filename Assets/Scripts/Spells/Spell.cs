using System;
using System.Collections.Generic;
using System.Linq;
using Creature;
using Inventory;
using Spells.Modifiers;
using UnityEngine;

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
        
        [field: SerializeField] public string Name { get; set; } // todo ComputedName based on modifiers?

        [field: SerializeField] public string Description { get; set; }

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

        public Spell(SpellStats baseStats, Element element, Team team)
        {
            BaseStats = baseStats;
            Element = element;
            Team = team;
            
            LastCastTime = -ComputedStats.CastCooldown;
            
            Name = $"{Element} Spell";
            Description = $"A {Element} spell";
        }
        
        public void AddModifier(SpellModifier modifier)
        {
            Modifiers.Add(modifier);
        }
        
        public void RemoveModifier(SpellModifier modifier)
        {
            Modifiers.Remove(modifier);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Creature;
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

        public static SpellStats operator +(SpellStats a, SpellStats b) =>
            new()
            {
                DamageOnHit = a.DamageOnHit + b.DamageOnHit,
                CastCooldown = a.CastCooldown + b.CastCooldown,
                ManaUsage = a.ManaUsage + b.ManaUsage,
                ProjectileSpeed = a.ProjectileSpeed + b.ProjectileSpeed,
                Range = a.Range + b.Range
            };

        public static SpellStats operator *(SpellStats a, SpellStats b) =>
            new()
            {
                DamageOnHit = a.DamageOnHit * b.DamageOnHit,
                CastCooldown = a.CastCooldown * b.CastCooldown,
                ManaUsage = a.ManaUsage * b.ManaUsage,
                ProjectileSpeed = a.ProjectileSpeed * b.ProjectileSpeed,
                Range = a.Range * b.Range
            };
    }

    [Serializable]
    public class Spell : IInventoryItem
    {
        [field: SerializeField] public string Name { get; set; } // todo ComputedName based on modifiers?
        [field: SerializeField] public Sprite Icon { get; set; }

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
using System;
using System.Collections.Generic;
using System.Linq;
using Creature;
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
    public class SpellStats
    {
        public float DamageOnHit;
        public float CastCooldown;
        public float ManaUsage;
        public float ProjectileSpeed;
        public float Range;
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

        public readonly List<SpellModifier> Modifiers = new();
        public Element Element;
        
        public float LastCastTime;
        
        public bool CooldownOver => Time.time - LastCastTime > ComputedStats.CastCooldown;
        
        public Spell(SpellStats baseStats, Element element, Team team)
        {
            BaseStats = baseStats;
            Element = element;
            Team = team;
        }
    }
}
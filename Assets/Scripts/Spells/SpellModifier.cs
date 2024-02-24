using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Spells
{
    namespace Modifiers
    {
        public enum ModifierTier
        {
            Tier1, // temp names
            Tier2,
            Tier3
        }

        public enum StatModifierMethod
        {
            Additive,
            Multiplicative
        }

        // todo
        // public enum MovementPattern
        // {
        //     None,
        //     SineWave,
        //     Sawtooth,
        // }

        [CreateAssetMenu(fileName = "SpellModifier", menuName = "Spells/Modifiers/SpellModifier", order = 0)]
        public class SpellModifier : ScriptableObject, IInventoryItem
        {
            public static List<SpellModifier> AllModifiers => Resources.LoadAll<SpellModifier>("Modifiers").ToList();

            [field: SerializeField] public string Name { get; set; }
            [field: SerializeField] public string Description { get; set; }
            [field: SerializeField] public Sprite Icon { get; set; }

            public ModifierTier Tier;

            [Space] public bool Stats;

            [InfoBox("Additive adds to stats, multiplicative multiplies the stats together")] [ShowIf("Stats")] [Space]
            public StatModifierMethod ModifierMethod;

            [InfoBox("Stats applied to the spell, can be negative")] [ShowIf("Stats")]
            public SpellStats StatsModifiers;

            public bool ExplodeOnHit;

            [InfoBox("Radius in units of explosion")] [ShowIf("ExplodeOnHit")]
            public float ExplosionRadius;

            public bool Piercing;

            [InfoBox("Number of enemies to pass through before dissipating")] [ShowIf("Piercing")]
            public int PierceTimes;

            public bool Bouncing;

            [InfoBox("Number of bounces before dissipating")] [ShowIf("Bouncing")]
            public int BounceTimes;

            public bool ExtraProjectiles;

            [InfoBox("Number of extra projectiles to spawn")] [ShowIf("ExtraProjectiles")]
            public int ExtraProjectilesAmount;

            [ShowIf("ExtraProjectiles")] [InfoBox("Angle in degrees between projectile trajectories")]
            public float ExtraProjectilesSpreadDegrees;

            // todo
            // public bool AlterMovementPattern;
            //
            // [InfoBox("Movement pattern to apply to the projectile")] [ShowIf("AlterMovementPattern")]
            // public MovementPattern MovementPattern;

            public SpellStats ModifyStats(SpellStats currentStats)
            {
                if (!Stats) return currentStats;

                if (ModifierMethod == StatModifierMethod.Additive)
                {
                    return currentStats + StatsModifiers;
                }
                else
                {
                    return currentStats * StatsModifiers;
                }
            }
        }
    }
}
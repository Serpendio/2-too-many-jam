using System.Collections.Generic;
using System.Linq;
using Inventory;
using NaughtyAttributes;
using UnityEngine;

namespace Spells
{
    namespace Modifiers
    {
        public enum ModifierTier
        {
            Tier1,
            Tier2,
            Tier3
        }

        public enum StatModifierMethod
        {
            Additive,
            Multiplicative
        }

        [CreateAssetMenu(fileName = "SpellModifier", menuName = "Spells/Modifiers/SpellModifier", order = 0)]
        public class SpellModifier : ScriptableObject, IInventoryItem
        {
            public static List<SpellModifier> AllModifiers => Resources.LoadAll<SpellModifier>("Modifiers").ToList();

            [field: SerializeField] public string Name { get; set; }
            [field: SerializeField] public string Description { get; set; }
            [field: SerializeField] public Sprite Icon { get; set; }

            public int GridIndex { get; set; } = -1;

            public bool SetDescriptive;

            [ShowIf(nameof(SetDescriptive))] public string Verb;

            public bool SetNoun;

            [ShowIf(nameof(SetNoun))] public string Noun;
            
            public ModifierTier Tier;

            [Space, HorizontalLine, Space] public bool Stats;

            [InfoBox("Additive adds to stats, multiplicative multiplies the stats together"), ShowIf(nameof(Stats))]
            public StatModifierMethod ModifierMethod;

            [InfoBox("Stats applied to the spell, can be negative"), ShowIf(nameof(Stats))]
            public SpellStats StatsModifiers;

            public bool ExplodeOnHit;

            [InfoBox("Radius in units of explosion"), ShowIf(nameof(ExplodeOnHit))]
            public float ExplosionRadius;

            [ShowIf(nameof(ExplodeOnHit))] public float TimeToLive;

            public bool Piercing;

            [InfoBox("Number of enemies to pass through before dissipating"), ShowIf(nameof(Piercing))]
            public int PierceTimes;

            public bool Bouncing;

            [InfoBox("Number of bounces before dissipating"), ShowIf(nameof(Bouncing))]
            public int BounceTimes;

            public bool ExtraProjectiles;

            [InfoBox("Number of extra projectiles to spawn"), ShowIf(nameof(ExtraProjectiles))]
            public int ExtraProjectilesAmount;

            [InfoBox("Angle in degrees between projectile trajectories"), ShowIf("ExtraProjectiles")]
            public float ExtraProjectilesSpreadDegrees;

            public bool Chain;

            [InfoBox("Number of chains between enemies before dissipating"), ShowIf(nameof(Chain))]
            public int ChainTimes;

            [InfoBox("Maximum radius for a possible chain"), ShowIf(nameof(Chain))]
            public float ChainRadius;

            public bool Homing;

            [InfoBox("Change in homing angle per second"), ShowIf(nameof(Homing))]
            public float DeltaHomingAngle;

            public bool Giant;

            [InfoBox("Changes size of the spell"), ShowIf(nameof(Giant))]
            public float ExtraSize;

            public bool BurstFire;

            [InfoBox("How many shots will be fired"), ShowIf(nameof(BurstFire))]
            public int HowManyShots;

            public bool Orbital;

            [InfoBox("Radius of the orbiting shot"), ShowIf(nameof(Orbital))]
            public float ShotRadius;

            public bool Tornado;

            [InfoBox("Pulling power of the tornado"), ShowIf(nameof(Tornado))]
            public float PullPower;

            [InfoBox("radius of pull"), ShowIf(nameof(Tornado))]
            public float PullRadius;

            public bool Barrier;

            [InfoBox("Width of the barrier"), ShowIf(nameof(Barrier))]
            public float SizeOfBarrier;

            public SpellStats ModifyStats(SpellStats currentStats)
            {
                if (!Stats) return currentStats;

                if (ModifierMethod == StatModifierMethod.Additive)
                {
                    return currentStats + StatsModifiers;
                }

                return currentStats * StatsModifiers;
            }
        }
    }
}
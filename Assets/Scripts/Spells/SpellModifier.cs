using Creature;
using System;

namespace Spells
{
    public enum ModifierTier
    {
        Tier1, // temp names
        Tier2,
        Tier3
    }
    
    [Serializable]
    public abstract class SpellModifier
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ModifierTier Tier { get; }

        public abstract SpellStats ModifyStats(SpellStats currentStats);

        public abstract void Update(SpellProjectile projectile);

        public abstract void OnCollision(CreatureBase creature);
    }
}

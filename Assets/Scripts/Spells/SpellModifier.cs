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
        public string Name;
        public string Description;
        public ModifierTier Tier;

        public virtual SpellStats ModifyStats(SpellStats currentStats) {
            // ...
            return currentStats;
        }
    }
}

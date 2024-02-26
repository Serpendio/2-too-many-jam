using Creature;
using UnityEngine;

namespace Spells
{
    namespace Modifiers
    {
        public class ExplosiveModifier : SpellModifier
        {
            public override string Name => "Explosive";

            public override string Description => "Explosive.";

            public override ModifierTier Tier => ModifierTier.Tier1;

            public override Element Element => Element.Neutral;

            public override SpellStats ModifyStats(SpellStats currentStats)
            {
                return currentStats;
            }

            public override void OnCollision(CreatureBase creature)
            {
                return;
            }

            public override void Update(SpellProjectile projectile)
            {
                return;
            }
        }
    }
}

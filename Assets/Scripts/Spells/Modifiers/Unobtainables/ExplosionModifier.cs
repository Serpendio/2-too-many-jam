using Creature;
using UnityEngine;

namespace Spells
{
    namespace Modifiers
    {
        public class ExplosionModifier : SpellModifier
        {
            public override string Name => "ExplosionModifier";

            public override string Description => "ExplosionModifier.";

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

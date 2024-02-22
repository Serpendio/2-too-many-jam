using Creature;
using UnityEngine;

namespace Spells
{
    namespace Modifiers
    {
        public class SineModifier : SpellModifier
        {
            float t = Mathf.PI / 2f;
            const float magnitude = .001f;

            public override string Name => "Sine Wave";

            public override string Description => "Causes the projectile to move in a sine wave pattern.";

            public override ModifierTier Tier => ModifierTier.Tier1;

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
                t += Time.deltaTime;
                projectile.transform.position += (Vector3)(magnitude * Mathf.Sin(t) * Vector2.Perpendicular(projectile.GetComponent<Rigidbody2D>().velocity));
                return;
            }
        }
    }
}

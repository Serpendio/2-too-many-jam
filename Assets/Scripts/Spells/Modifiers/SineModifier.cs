using Creature;
using Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SineModifier : SpellModifier
{
    float t = Mathf.PI / 2f;
    const float magnitude = .00015f;

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

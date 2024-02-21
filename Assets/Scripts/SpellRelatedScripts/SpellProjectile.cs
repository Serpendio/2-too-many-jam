using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    Spell assignedSpell;
    bool team;
    Vector2 baseVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.collider.CompareTag("Creature") && col.collider.TryGetComponent(out CreatureBase creature))
        {
            if (creature.team != team) {
                Stats totalStats = new();
                Stats baseStats = assignedSpell.stats;
                List<SpellModifier> mods = assignedSpell.spellMods;

                totalStats = mods.Aggregate(baseStats, (spellStats, modifier) => modifier.modifyStats(spellStats));

                creature.Damage(totalStats.Damage);
            }
        }
    }
    
}
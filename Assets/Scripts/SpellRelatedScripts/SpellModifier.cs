using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class SpellModifier
{
    string name;
    string desc;
    int level;
    Stats stats;
    

    virtual public void Update(SpellProjectile proj) {

    }

    virtual public Stats modifyStats(Stats currentStats) {
        currentStats.Damage += stats.Damage;

        return currentStats;
    }
}

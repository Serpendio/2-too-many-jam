using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInventory : MonoBehaviour
{
    public List<Spell> spells = new(10) {new Spell(5, 10)};
    public Spell currentSpell;

    void Awake() {
        currentSpell = spells[0];
    }
}
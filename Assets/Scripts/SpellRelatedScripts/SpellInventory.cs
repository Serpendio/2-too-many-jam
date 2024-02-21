using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpellInventory : MonoBehaviour
{
    List<Spell> spells = new(10) {new Spell(5, 10)};
    int currentSpell;

    void Awake() {
        currentSpell = 0;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("SpellPickup")) {
            spells.Add(spellRand());
            foreach (Spell spell in spells) {
                Debug.Log(spell.element);
            }
        }
    }

    Spell spellRand() {
        Spell newSpell = new();

        float rNumber = UnityEngine.Random.Range(0f, 100f);
        if (rNumber <= 50) {
            newSpell.element = Element.none;
        } else if (rNumber <= 60) {
            newSpell.element = Element.Fire;
        } else if (rNumber <= 70) {
            newSpell.element = Element.Water;
        } else if (rNumber <= 80) {
            newSpell.element = Element.Lightning;
        } else if (rNumber <= 90) {
            newSpell.element = Element.Air;
        } else if (rNumber <= 100) {
            newSpell.element = Element.Earth;
        } else  {
            Debug.Log("ERROR - Random number generation bad");
        }

        // Rounds to 1 decimals
        newSpell.castSpeed = math.round(UnityEngine.Random.Range(1.2f, 3f) * 10) / 10;

        newSpell.manaUsage = UnityEngine.Random.Range(2, 10);

        return newSpell;
    }
}
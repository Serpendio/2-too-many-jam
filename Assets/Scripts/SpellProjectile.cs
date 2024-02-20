using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    GameObject projectile;
    private int mana;
    private float castCooldown;

    Spell spell = new(1.2f, 2, Element.Fire);
    void onShoot() 
    {
        GameObject spellShot = Instantiate(projectile, transform.position, transform.rotation);
        spellShot.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 10f, 0));
    }
}

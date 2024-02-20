using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellProjectile : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] private int mana;
    [SerializeField] private float castCooldown;

    public InputMaster controls;

    Spell spell = new(1.2f, 2, Element.Fire);

    void Awake() {
        // Need to create instance of inputs object - MUST be done first thing
        controls = new InputMaster();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Update() 
    {
        if (controls.Player.Shoot.triggered) {
            GameObject spellShot = Instantiate(projectile, transform.position, transform.rotation);
            spellShot.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector3(0, 10f, 0));
        }
    }
}

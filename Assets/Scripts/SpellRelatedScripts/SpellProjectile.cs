using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
            Vector2 direction = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject spellShot = Instantiate(projectile, transform.position, transform.rotation);

            spellShot.GetComponent<Transform>().rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
            spellShot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            spellShot.GetComponent<Rigidbody2D>().AddForce(spellShot.GetComponent<Transform>().up * 500);
            Destroy(spellShot, 5.0f);
        }
    }
}

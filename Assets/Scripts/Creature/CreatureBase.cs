using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBase : MonoBehaviour
{
    [SerializeField] protected float maxHealth { get; set; }
    [SerializeField] protected float attackCooldown { get; set; }
    [SerializeField] protected float attackRange { get; set; }
    [SerializeField] protected float attackDamage { get; set; }
    [SerializeField, Min(0)] private float speed;
    private float health, cooldown;

    protected virtual void Awake()
    {
        health = maxHealth;
        cooldown = 0;
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else if (health > maxHealth) // to allow for healing
        {
            health = maxHealth;
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

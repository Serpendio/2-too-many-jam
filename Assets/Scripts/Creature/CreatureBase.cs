using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBase : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float speed;
    private float health, cooldown;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] protected bool isFacingRight;

    protected virtual void Awake()
    {
        health = maxHealth;
        cooldown = 0;
    }

    protected void UpdateDir(Vector2 moveDir)
    {
        spriteRenderer.flipX = !isFacingRight;
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

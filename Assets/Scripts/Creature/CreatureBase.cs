using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBase : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float speed;
    private float health;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    protected void UpdateDir(Vector2 moveDir)
    {
        if (moveDir.sqrMagnitude != 0)
        {
            spriteRenderer.flipX = moveDir.x > 0;
        }

        anim.SetFloat("xMove", moveDir.x);
        anim.SetFloat("yMove", moveDir.y);
    }

    protected virtual void Attack()
    {
        anim.SetTrigger("Attack");
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBase : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float speed;
    [SerializeField] private float health;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;
    public bool team;

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    protected void UpdateDir(Vector2 lookDir, bool isMoving)
    {
        anim.SetFloat("multiplier", Convert.ToInt32(isMoving));

        if (lookDir.sqrMagnitude == 0)
        {
            return;
        }

        if (lookDir.sqrMagnitude != 0)
        {
            spriteRenderer.flipX = lookDir.x < 0;
        }

        anim.SetFloat("xMove", lookDir.x);
        anim.SetFloat("yMove", lookDir.y);
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

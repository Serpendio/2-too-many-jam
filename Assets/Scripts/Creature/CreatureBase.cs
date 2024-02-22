using UnityEngine;
using System;

namespace Creature
{
    public enum Team
    {
        Friendly,
        Hostile
    }

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CreatureBase : MonoBehaviour
    {
        private static readonly int XMove = Animator.StringToHash("xMove");
        private static readonly int YMove = Animator.StringToHash("yMove");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Multiplier = Animator.StringToHash("multiplier");

        protected Animator Anim;
        protected SpriteRenderer SpriteRenderer;
        protected Rigidbody2D Rb;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected float speed;
        [SerializeField] private float health;

        public Team Team;

        protected virtual void Awake()
        {
            health = maxHealth;

            Anim = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rb = GetComponent<Rigidbody2D>();
        }

        protected void UpdateMoveDir(Vector2 lookDir, bool isMoving)
        {
            Anim.SetFloat(Multiplier, Convert.ToInt32(isMoving));

            if (lookDir.sqrMagnitude == 0)
            {
                return;
            }

            if (lookDir.sqrMagnitude != 0)
            {
                SpriteRenderer.flipX = lookDir.x < 0;
            }

            Anim.SetFloat(XMove, lookDir.x);
            Anim.SetFloat(YMove, lookDir.y);
        }

        // protected virtual void Attack()
        // {
        //     Anim.SetTrigger(Attack1);
        // }

        public virtual void TakeDamage(float damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);
            if (health == 0) Die();
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}
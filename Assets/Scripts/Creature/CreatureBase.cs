using UnityEngine;
using System;
using Tweens;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Creature
{
    public enum Team
    {
        Friendly,
        Hostile,
        DamagesAll
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

        [HideInInspector] public UnityEvent OnDeath = new();

        protected Animator Anim;
        protected SpriteRenderer SpriteRenderer;
        [HideInInspector] public Rigidbody2D Rb;

        [SerializeField] private float health;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float moveSpeed;

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

        protected virtual void TriggerAttackAnim()
        {
            Anim.SetTrigger(Attack);
        }

        public virtual void TakeDamage(float damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);

            // tween flash sprite colour as red
            gameObject.AddTween(new SpriteRendererColorTween
            {
                from = Color.white,
                to = new Color(1f, 0.2f, 0.2f, 0.6667f),
                duration = 0.05f,
                easeType = EaseType.CubicInOut,
                usePingPong = true
            });

            if (health == 0) Die();
        }

        protected virtual void Die()
        {
            OnDeath.Invoke();
            Destroy(gameObject);
        }
    }
}
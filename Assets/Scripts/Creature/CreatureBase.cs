using UnityEngine;
using System;
using Tweens;
using UI;
using UnityEngine.Events;

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

        private static HealthChangeIndicator _healthChangeIndicatorPrefab;
        
        [HideInInspector] public UnityEvent OnDeath = new();

        protected Animator Anim;
        protected SpriteRenderer spriteRenderer;
        [HideInInspector] public Rigidbody2D Rb;

        [field: SerializeField] public float health { get; private set; }
        [field: SerializeField] public float maxHealth { get; private set; }

        [HideInInspector] public UnityEvent<float, float> OnHealthChanged = new();

        public Team Team;
        
        protected virtual void Awake()
        {
            Core.Locator.CreatureManager.AddCreature(this);

            health = maxHealth;

            Anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            Rb = GetComponent<Rigidbody2D>();
            
            if (!_healthChangeIndicatorPrefab) _healthChangeIndicatorPrefab = Resources.Load<HealthChangeIndicator>("Prefabs/HealthChangeIndicator");
        }

        protected virtual void Start()
        {
            SetHealth(maxHealth);
        }

        public void SetHealth(float value, bool showIndicator = false)
        {
            var diff = value - health;
            
            health = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged.Invoke(health, maxHealth);
            
            if (showIndicator && diff != 0)
            {
                var indicator = Instantiate(_healthChangeIndicatorPrefab, transform.position, Quaternion.identity);
                indicator.Change = diff;
            }
        }

        public void SetMaxHealth(float value, bool refill = false)
        {
            var diff = value - maxHealth;
            maxHealth = value;
            if (refill)
            {
                SetHealth(health + diff);
            }
            else
            {
                OnHealthChanged.Invoke(health, maxHealth);
            }
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
                spriteRenderer.flipX = lookDir.x < 0;
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
            SetHealth(Mathf.Clamp(health - damage, 0, maxHealth), showIndicator: true);
            
            if (health == 0)
            {
                Die();
                return;
            }
            
            // tween flash sprite colour as red
            gameObject.AddTween(new SpriteRendererColorTween
            {
                from = Color.white,
                to = new Color(1f, 0.2f, 0.2f, 0.6667f),
                duration = 0.05f,
                easeType = EaseType.CubicInOut,
                usePingPong = true,
                onEnd = _ => spriteRenderer.color = Color.white,
            });
        }

        public virtual void RefillHealth()
        {
            SetHealth(maxHealth);
        }

        protected virtual void Die()
        {
            OnDeath.Invoke();
            Destroy(gameObject);
        }

        [ContextMenu("Force Kill")]
        private void ForceKill()
        {
            SetHealth(0);
        }
    }
}
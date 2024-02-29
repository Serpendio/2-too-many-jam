using UnityEngine;
using System;
using Tweens;
using UI;
using UnityEngine.Events;
using System.Collections;

namespace Creature
{
    public enum Team
    {
        Friendly,
        Hostile,
        DamagesAll
    }

    [HideInInspector] public enum Debuff
    {
        Poison
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


        //----TO ADD A NEW DEBUFF:----//
        //*Add the name of the debuff to the Debuff enum
        //*Create a new private IEnumerator method with up to 3 parameters to house the debuff logic in
        //*Add the method to the debuffCoroutines array in Start()

        //----TO CALL A DEBUFF----//
        //*Call CreatureBase.ApplyDebuff(Debuff debuff, float param1, float param2, float param3)

        //----TO CANCEL A DEBUFF PREMATURELY----//
        //*Call RemoveDebuff(Debuff debuff)

        public bool[] activeDebuffs;
        //Helper-delegate, just used to pass more than one parameter to the coroutines :]
        //Default values provided in case coroutine need not take all 3 parameters
        public delegate IEnumerator MultiParamCoroutine(float param1=0, float param2=0, float param3=0);
        public MultiParamCoroutine[] debuffCoroutines;
        
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
            activeDebuffs = new bool[Enum.GetNames(typeof(Debuff)).Length];
            debuffCoroutines = new MultiParamCoroutine[] {Poison};
        }

        protected IEnumerator Poison(float damagePerHit, float secondsToRunFor, float secondsBetweenPoisonHits)
        {
            float totalTime = 0f;
            //Run for specified number of seconds
            while (totalTime < secondsToRunFor && activeDebuffs[(int)Debuff.Poison])
            {
                //Apply poison effect
                health -= damagePerHit;
                totalTime += secondsBetweenPoisonHits;
                yield return new WaitForSeconds(secondsBetweenPoisonHits);
            }
            yield break;
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

        public virtual void ApplyDebuff(Debuff debuff, float param1, float param2, float param3) {
            activeDebuffs[(int)debuff] = true;
            StartCoroutine(debuffCoroutines[(int)debuff](param1, param2, param3));
        }

        //Prematurely end debuff
        public virtual void RemoveDebuff(Debuff debuff) {
            activeDebuffs[(int)debuff] = false;
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
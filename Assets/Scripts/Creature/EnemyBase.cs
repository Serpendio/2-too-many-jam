using Core;
using Currency;
using NaughtyAttributes;
using Spells;
using Tweens;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Creature
{
    public enum EnemyType
    {
        Melee,
        Spell
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBase : CreatureBase
    {
        [FormerlySerializedAs("agent")] [SerializeField]
        public NavMeshAgent Agent;

        [HideInInspector] public CreatureBase target;

        // [SerializeField] ScriptableObject enemyData;

        [SerializeField] protected EnemyType EnemyType;

        [ShowIf("EnemyType", EnemyType.Melee)] [SerializeField]
        protected float attackCooldown;

        [ShowIf("EnemyType", EnemyType.Melee)] [SerializeField]
        protected float attackRange;

        [ShowIf("EnemyType", EnemyType.Melee)] [SerializeField]
        protected float attackDamage;

        [ShowIf("EnemyType", EnemyType.Spell)] [SerializeField]
        protected Spell Spell;

        private float LastAttackTime;

        private static CoinDrop _coinDropPrefab;
        // private bool _isAttacking = false;

        [SerializeField] private float targetingRange;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            Agent.enabled = false;
            LastAttackTime = -attackCooldown;

            if (_coinDropPrefab == null) _coinDropPrefab = Resources.Load<CoinDrop>("Prefabs/CoinDrop");
        }

        protected override void Start()
        {
            Agent.enabled = true;

            // if (Random.value > 0.4f) EnemyType = EnemyType.Melee;
        }

        private void Attack()
        {
            TriggerAttackAnim();
            // _isAttacking = true;

            switch (EnemyType)
            {
                case EnemyType.Melee:
                    var endPoint = target.transform.position +
                                   (target.transform.position - transform.position).normalized * attackRange;
                    gameObject.AddTween(new LocalPositionTween
                    {
                        from = transform.position,
                        to = endPoint,
                        duration = (endPoint - transform.position).magnitude / 6,
                        easeType = EaseType.CubicInOut,
                        usePingPong = false,
                        // onEnd = _ =>
                        // {
                        //     _isAttacking = false;
                        // }
                    });
                    break;
                case EnemyType.Spell:
                    // Get direction from player to mouse
                    var dir = (target.transform.position - transform.position).normalized;

                    var overseer = gameObject.AddComponent<SpellProjectileOverseer>();
                    overseer.Spell = Spell;
                    overseer.CastDirection = dir;
                    break;
            }
        }

        protected override void Die()
        {
            base.Die();

            if (Random.value > Locator.GameplaySettingsManager.CoinDropChance) return;

            var coinDrop = Instantiate(_coinDropPrefab, transform.position, Quaternion.identity);
            coinDrop.coinValue = Mathf.RoundToInt(Locator.GameplaySettingsManager.CoinDropValue.GetValue());
        }

        // Update is called once per frame
        private void Update()
        {
            // if animation currently playing is Attack, don't move
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                Agent.destination = transform.position;
            }
            else
                Agent.destination = target.transform.position;

            var distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            var hasLineOfSight = !Physics2D.Linecast(
                transform.position,
                target.transform.position,
                LayerMask.GetMask("Wall")
            );

            UpdateMoveDir(Agent.velocity, Agent.velocity.sqrMagnitude > 0);

            switch (EnemyType)
            {
                case EnemyType.Melee:
                    Agent.stoppingDistance = hasLineOfSight
                        ? attackRange
                        : 0;

                    if (hasLineOfSight && distanceToTarget < attackRange && Time.time - LastAttackTime > attackCooldown)
                    {
                        Attack();
                        LastAttackTime = Time.time;
                    }

                    break;
                case EnemyType.Spell:
                    Agent.stoppingDistance = hasLineOfSight
                        ? Spell.ComputedStats.Range
                        : 0;

                    if (hasLineOfSight && distanceToTarget < Spell.ComputedStats.Range && Spell.CooldownOver)
                    {
                        Attack();
                        Spell.LastCastTime = Time.time;
                    }

                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (EnemyType == EnemyType.Melee && collision.gameObject.CompareTag("Room"))
            {
                gameObject.CancelTweens();
            }

            if (collision.gameObject.TryGetComponent(out CreatureBase creature) && creature.Team != Team)
            {
                creature.TakeDamage(attackDamage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, EnemyType == EnemyType.Melee ? attackRange : Spell.ComputedStats.Range);
        }
    }
}
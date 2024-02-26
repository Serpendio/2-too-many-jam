using System;
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

        [SerializeField] private float targetingRange;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            Agent.enabled = false;
            
            if (_coinDropPrefab == null) _coinDropPrefab = Resources.Load<CoinDrop>("Prefabs/CoinDrop");
        }

        protected void Start()
        {
            Agent.enabled = true;

            // if (Random.value > 0.4f) EnemyType = EnemyType.Melee;
        }

        private void Attack()
        {
            TriggerAttackAnim();

            if (EnemyType == EnemyType.Melee)
            {
                gameObject.AddTween(new LocalPositionTween
                {
                    from = transform.position,
                    to = transform.position + (target.transform.position - transform.position).normalized * 0.25f,
                    duration = 0.05f,
                    easeType = EaseType.ExpoInOut,
                    usePingPong = true
                });

                target.TakeDamage(attackDamage);
            }
            else if (EnemyType == EnemyType.Spell)
            {
                // Get direction from player to mouse
                var dir = (target.transform.position - transform.position).normalized;

                var overseer = gameObject.AddComponent<SpellProjectileOverseer>();
                overseer.Spell = Spell;
                overseer.CastDirection = dir;
            }
        }

        protected override void Die()
        {
            base.Die();

            if (Random.value <= Locator.GameplaySettingsManager.CoinDropChance)
            {
                var coinDrop = Instantiate(_coinDropPrefab, transform.position, Quaternion.identity);
                coinDrop.coinValue = Mathf.RoundToInt(Locator.GameplaySettingsManager.CoinDropValue.GetValue());
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            // if animation currently playing is Attack, don't move
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                Agent.velocity = Vector3.zero;
            }

            var distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            Agent.SetDestination(target.transform.position);
            if (Agent.remainingDistance > targetingRange) { Agent.velocity = Vector3.zero; }
            UpdateMoveDir(target.transform.position - transform.position, Agent.velocity.sqrMagnitude > 0);

            if (EnemyType == EnemyType.Melee)
            {
                if (distanceToTarget <= attackRange && Time.time - LastAttackTime > attackCooldown)
                {
                    Attack();
                    LastAttackTime = Time.time;
                }
            }
            else if (EnemyType == EnemyType.Spell)
            {
                var hasLineOfSight = !Physics2D.Linecast(
                    transform.position,
                    target.transform.position,
                    LayerMask.GetMask("Wall")
                );

                Agent.stoppingDistance = hasLineOfSight
                    ? Spell.ComputedStats.Range
                    : 0;

                if (hasLineOfSight && distanceToTarget < Spell.ComputedStats.Range && Spell.CooldownOver)
                {
                    Attack();
                    Spell.LastCastTime = Time.time;
                }
            }
        }
    }
}
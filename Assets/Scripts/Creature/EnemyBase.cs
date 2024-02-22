using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : CreatureBase
{
    [SerializeField] ScriptableObject enemyData;
    [SerializeField] private Transform target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;
    private float cooldown;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        cooldown = 0;
        agent.stoppingDistance = 1;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
        UpdateDir(target.position - transform.position, agent.velocity.sqrMagnitude > 0);
    }
}

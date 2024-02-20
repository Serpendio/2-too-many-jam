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

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
        agent.stoppingDistance = 1;
    }
}

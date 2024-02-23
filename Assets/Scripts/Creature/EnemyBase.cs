using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Creature
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBase : CreatureBase
    {
        [FormerlySerializedAs("agent")] [SerializeField] public NavMeshAgent Agent;
        public Transform target;
        
        [SerializeField] ScriptableObject enemyData;
        
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackDamage;
        private float cooldown;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            cooldown = 0;
            Agent.stoppingDistance = 1;
            Agent.enabled = false;
        }

        protected void Start()
        {
            Agent.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            Agent.SetDestination(target.position);
            UpdateMoveDir(target.position - transform.position, Agent.velocity.sqrMagnitude > 0);
        }
    }
}
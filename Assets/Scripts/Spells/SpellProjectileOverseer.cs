using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rooms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spells
{
    public class SpellProjectileOverseer : MonoBehaviour
    {
        public static SpellProjectile SpellProjectilePrefab;

        public Spell Spell;

        public Vector2 CastDirection;
        public List<SpellProjectile> Projectiles = new();

        bool isCreateProjRunning;

        private IEnumerator CreateProjectile(Vector2 castDirection)
        {
            isCreateProjRunning = true;
            int BurstShots = Spell.Modifiers.Where(m => m.BurstFire).Sum(m => m.HowManyShots) + 1;
            var lastMouseOffset = CastDirection;

            for (int i = 0; i < BurstShots; i++)
            {
                // Get direction from player to mouse
                if (transform.CompareTag("Player"))
                {
                    var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                    float newOffset = Vector2.SignedAngle(lastMouseOffset, mousePos - transform.position);
                    castDirection = Quaternion.Euler(0, 0, newOffset) * castDirection;
                    lastMouseOffset = mousePos - transform.position;
                }

                var projectile = Instantiate(SpellProjectilePrefab, transform.position, Quaternion.identity);
                Projectiles.Add(projectile);

                projectile.Overseer = this;
                projectile.Spell = Spell;
                projectile.CastDirection = castDirection;
                projectile.BouncesRemaining = Spell.Modifiers.Where(m => m.Bouncing).Sum(m => m.BounceTimes);
                projectile.PiercesRemaining = Spell.Modifiers.Where(m => m.Piercing).Sum(m => m.PierceTimes);
                projectile.ChainTimesRemaining = Spell.Modifiers.Where(m => m.Chain).Sum(m => m.ChainTimes);
                projectile.ChainSqrRadius = Spell.Modifiers.Where(m => m.Chain).Sum(m => m.ChainRadius);
                projectile.ChainSqrRadius *= projectile.ChainSqrRadius;
                projectile.HomingAngle = Spell.Modifiers.Where(m => m.Homing).Sum(m => m.DeltaHomingAngle);
                projectile.GiantSize = Spell.Modifiers.Where(m => m.Giant).Sum(m => m.ExtraSize);
                projectile.ExplodeRad = Spell.Modifiers.Where(m => m.ExplodeOnHit).Sum(m => m.ExplosionRadius);
                projectile.AliveTime = Spell.Modifiers.Where(m => m.ExplodeOnHit).Sum(m => m.TimeToLive);
                projectile.OrbitRadius = Spell.Modifiers.Where(m => m.Orbital).Sum(m => m.ShotRadius);
                projectile.TornadoPower = Spell.Modifiers.Where(m => m.Tornado).Sum(m => m.PullPower);
                projectile.TornadoRadius = Spell.Modifiers.Where(m => m.Tornado).Sum(m => m.PullRadius);
                projectile.BarrierSize = Spell.Modifiers.Where(m => m.Barrier).Sum(m => m.SizeOfBarrier);

                yield return new WaitForSeconds(.08f);
            }

            isCreateProjRunning = false;
        }

        private void Awake()
        {
            if (SpellProjectilePrefab == null)
                SpellProjectilePrefab = Resources.Load<SpellProjectile>("Prefabs/SpellProjectile");
            Room.OnEnteredRoom.AddListener(_ => Destroy(this));
        }

        private void Start()
        {
            int projectilesAmount = Spell.Modifiers
                .Where(m => m.ExtraProjectiles)
                .Sum(m => m.ExtraProjectilesAmount) + 1;
            float projectilesSpreadDegrees = Spell.Modifiers
                .Where(m => m.ExtraProjectiles)
                .Sum(m => m.ExtraProjectilesSpreadDegrees) + Spell.ComputedStats.Spread;

            if (projectilesAmount == 1)
            {
                StartCoroutine(CreateProjectile(CastDirection));
            }
            else
            {
                for (var i = 0; i < projectilesAmount; i++)
                {
                    var angleDiff = projectilesSpreadDegrees / (projectilesAmount - 1);
                    var offset = projectilesSpreadDegrees / 2;
                    Vector2 spreadDir = Quaternion.Euler(0, 0, angleDiff * i - offset) * CastDirection;

                    StartCoroutine(CreateProjectile(spreadDir));
                }
            }
        }

        public void RemoveProjectile(SpellProjectile projectile)
        {
            Projectiles.Remove(projectile);
            if (Projectiles.Count == 0 && !isCreateProjRunning) Destroy(this);
        }

        private void OnDestroy()
        {
            foreach (var projectile in Projectiles)
            {
                Destroy(projectile.gameObject);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Rooms;
using UnityEngine;

namespace Spells
{
    public class SpellProjectileOverseer : MonoBehaviour
    {
        public static SpellProjectile SpellProjectilePrefab;
        
        public Spell Spell;

        public Vector2 CastDirection;
        public List<SpellProjectile> Projectiles = new();

        private void CreateProjectile(Vector2 castDirection)
        {
            var projectile = Instantiate(SpellProjectilePrefab, transform.position + (Vector3.up * 0.5f), Quaternion.identity);
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
        }

        private void Awake()
        {
            if(SpellProjectilePrefab == null) SpellProjectilePrefab = Resources.Load<SpellProjectile>("Prefabs/SpellProjectile");
            Room.OnEnteredRoom.AddListener(_ => Destroy(this));
        }

        private void Start()
        {
            //CreateProjectile(CastDirection);

            int projectilesAmount = Spell.Modifiers.Where(m => m.ExtraProjectiles).Sum(m => m.ExtraProjectilesAmount) + 1;
            float projectilesSpreadDegrees = Spell.Modifiers.Where(m => m.ExtraProjectiles).Sum(m => m.ExtraProjectilesSpreadDegrees) + Spell.ComputedStats.Spread;
            if (projectilesAmount == 1)
            {
                var spreadDir = Quaternion.Euler(0, 0, Random.Range(-projectilesSpreadDegrees, projectilesSpreadDegrees)) * CastDirection;

                CreateProjectile(spreadDir);
            }
            else
            {
                for (var i = 0; i < projectilesAmount; i++)
                {
                    // given a center projectile P, rings are pairs of similarly-distanced projectiles on either side,
                    // i.e: 3\ 2\ 1\ |P /1 /2 /3
                    var angleDiff = projectilesSpreadDegrees / (projectilesAmount - 1);
                    var offset = projectilesSpreadDegrees / 2;
                    var spreadDir = Quaternion.Euler(0, 0, angleDiff * i - offset) * CastDirection;

                    CreateProjectile(spreadDir);
                }
            }
        }

        public void RemoveProjectile(SpellProjectile projectile)
        {
            Projectiles.Remove(projectile);
            if (Projectiles.Count == 0) Destroy(this);
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
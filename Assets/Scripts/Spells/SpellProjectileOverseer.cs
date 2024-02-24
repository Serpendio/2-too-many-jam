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
        }

        private void Awake()
        {
            if(SpellProjectilePrefab == null) SpellProjectilePrefab = Resources.Load<SpellProjectile>("Prefabs/SpellProjectile");
            Room.OnEnteredRoom.AddListener(_ => Destroy(this));
        }

        private void Start()
        {
            CreateProjectile(CastDirection);

            foreach (var modifier in Spell.Modifiers.Where(modifier => modifier.ExtraProjectiles))
            {
                for (var i = 0; i < modifier.ExtraProjectilesAmount; i++)
                {
                    // given a center projectile P, rings are pairs of similarly-distanced projectiles on either side,
                    // i.e: 3\ 2\ 1\ |P /1 /2 /3
                    var ring = (i / 2) + 1;
                    var spread = modifier.ExtraProjectilesSpreadDegrees;

                    var angleDiff = i % 2 == 0 ? -spread * ring : spread * ring;
                    var spreadDir = Quaternion.Euler(0, 0, angleDiff) * CastDirection;
                    
                    CreateProjectile(spreadDir);
                }
            }
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
using System;
using Creature;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spells
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(ParticleSystem))]
    public class SpellProjectile : MonoBehaviour
    {
        public SpellProjectileOverseer Overseer;
        
        private Rigidbody2D _rb;
        private ParticleSystem _particleSystem;

        public Spell Spell;
        public Vector2 CastDirection;

        public int BouncesRemaining;
        public int PiercesRemaining;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            if (Spell.Team == Team.Friendly) gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
            else gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");

            _rb.AddForce(Spell.ComputedStats.ProjectileSpeed * CastDirection, ForceMode2D.Impulse);

            var main = _particleSystem.main;

            // main.startColor = Spell.Element switch
            // {
            //     Element.None => Color.white,
            //     Element.Fire => Color.red,
            //     Element.Water => Color.blue,
            //     Element.Air => Color.cyan,
            //     Element.Lightning => Color.yellow,
            //     Element.Earth => Color.green,
            //     _ => Color.white
            // };
            
            // main.startColor = new Color(Random.Range(0.75f, 1f), Random.Range(0.75f, 1f), Random.Range(0.75f, 1f));

            var colours = new[] { Color.cyan, new Color(237/255f, 178/255f, 229/255f), Color.white };
            main.startColor = colours[Random.Range(0, colours.Length)];
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Creature") && col.collider.TryGetComponent(out CreatureBase creature))
            {
                if (creature.Team == Spell.Team) return;
                creature.TakeDamage(Spell.ComputedStats.DamageOnHit);

                if (PiercesRemaining > 0)
                { 
                    // note for future jowsey : Physics.IgnoreCollision for one frame here, worked in Freeroam
                    PiercesRemaining--;
                }
                else
                {
                    Dissipate();
                }
            }
            else if (col.collider.CompareTag("Room"))
            {
                if (BouncesRemaining > 0)
                {
                    BouncesRemaining--;
                }
                else
                {
                    Dissipate();
                }
            }
        }

        public void Dissipate()
        {
            _rb.simulated = false;
            _particleSystem.Stop();
            Destroy(gameObject, _particleSystem.main.startLifetime.constantMax);
        }

        private void OnDestroy()
        {
            Overseer.Projectiles.Remove(this);
        }
    }
}
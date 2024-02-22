using Creature;
using UnityEngine;

namespace Spells
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(ParticleSystem))]
    public class SpellProjectile : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private ParticleSystem _particleSystem;

        public Spell Spell;
        public Vector2 CastDirection;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            _rb.AddForce(Spell.ComputedStats.ProjectileSpeed * CastDirection, ForceMode2D.Impulse);

            var main = _particleSystem.main;

            main.startColor = Spell.Element switch
            {
                Element.None => Color.white,
                Element.Fire => Color.red,
                Element.Water => Color.blue,
                Element.Air => Color.cyan,
                Element.Lightning => Color.yellow,
                Element.Earth => Color.green,
                _ => Color.white
            };
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Creature") && col.collider.TryGetComponent(out CreatureBase creature))
            {
                if (creature.Team == Spell.Team) return;
                creature.TakeDamage(Spell.ComputedStats.DamageOnHit);
            }
        }
    }
}
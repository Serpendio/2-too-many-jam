using System;
using System.Linq;
using Core;
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

        private bool dissipated;

        private Rigidbody2D _rb;
        private ParticleSystem _particleSystem;

        public Spell Spell;
        public Vector2 CastDirection;

        public int BouncesRemaining;
        public int PiercesRemaining;
        public int ChainTimesRemaining;
        public float ChainSqrRadius;
        public float HomingAngle;
        public float GiantSize;
        public float ExplodeRad;
        public float AliveTime;
        public float OrbitRadius;
        public float TornadoPower;
        public float TornadoRadius;
        public float BarrierSize;

        public float TravelDistance;
        public float StartLive;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            StartLive = Time.time;

            if (BarrierSize > 0) transform.localScale += new Vector3(BarrierSize, 0, 0);
            if (GiantSize > 0) transform.localScale += new Vector3(GiantSize, GiantSize, 0);

            _rb.velocity = Spell.ComputedStats.ProjectileSpeed * CastDirection;
            
            if (OrbitRadius > 0) transform.position += (Vector3)CastDirection * OrbitRadius;

            _particleSystem.textureSheetAnimation.SetSprite(0, Spell.ElementSprites[Spell.Element]);
        }

        private void FixedUpdate()
        {
            if (OrbitRadius > 0)
            {
                // Calculate the angle between the player and the projectile
                Vector2 playerToProjectile = transform.position - Locator.Player.transform.position;
                float offsetAngle = Vector2.Angle(Vector2.right, playerToProjectile);

                // Add the offset angle to the rotation
                _rb.velocity = Quaternion.Euler(
                    0,
                    0,
                    -Mathf.Rad2Deg * Spell.ComputedStats.ProjectileSpeed * Time.fixedDeltaTime / OrbitRadius// + offsetAngle
                ) * _rb.velocity;

                //transform.position += (Vector3)Locator.Player.Rb.velocity * Time.fixedDeltaTime;
            }

            // rotate to face velocity
            var angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }

        private void Update()
        {
            if (AliveTime > 0 && Time.time - StartLive >= AliveTime)
            {
                Dissipate();
                return;
            }

            TravelDistance += _rb.velocity.magnitude * Time.deltaTime;

            if (TornadoPower > 0)
            {
                var creaturesToPull = FindObjectsByType<CreatureBase>(FindObjectsSortMode.None)
                    .Where(c => c is not Chest)
                    .Where(c => c.Team != Spell.Team)
                    .Where(c => (c.transform.position - transform.position).magnitude < TornadoRadius);
                foreach (var creature in creaturesToPull)
                {
                    creature.transform.position = Vector3.MoveTowards(
                        creature.transform.position,
                        transform.position,
                        TornadoPower * Time.deltaTime
                    );
                }
            }

            if (HomingAngle > 0)
            {
                var newCreature = FindObjectsByType<CreatureBase>(FindObjectsSortMode.None)
                    .Where(c => c.Team != Spell.Team)
                    .OrderBy(c => (c.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                if (newCreature != null)
                {
                    float angle =
                        Vector2.SignedAngle(_rb.velocity, newCreature.transform.position - transform.position);

                    if (angle > 0)
                    {
                        angle = MathF.Min(angle, HomingAngle * Time.deltaTime);
                    }
                    else if (angle < 0)
                    {
                        angle = MathF.Max(angle, -HomingAngle * Time.deltaTime);
                    }

                    _rb.velocity = Quaternion.Euler(0, 0, angle) * _rb.velocity;
                }
            }

            if (TravelDistance >= Spell.ComputedStats.Range) Dissipate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out SpellProjectile proj) && BarrierSize > 0)
            {
                if (proj.Spell.Team == Spell.Team) { return; }
                proj.Dissipate();
            }

            if (other.TryGetComponent(out CreatureBase creature))
            {
                if (creature.Team == Spell.Team) { return; } // better safe than sorry

                float damage = Spell.ComputedStats.DamageOnHit;
                //Apply elemental modifier
                if (Spell.Element == Element.Water) {
                    //Water beats fire
                    if (creature.element == Element.Fire)
                    {
                        damage *= 2;
                    }
                    //Water loses to lightning
                    else if (creature.element == Element.Lightning)
                    {
                        damage /= 2;
                    }
                }
                if (Spell.Element == Element.Earth) {
                    //Earth beats lightning
                    if (creature.element == Element.Lightning) {
                        damage *= 2;
                    }
                    //Earth loses to air
                    if (creature.element == Element.Air) {
                        damage /= 2;
                    }
                }
                if (Spell.Element == Element.Fire)
                {
                    //Fire beats air
                    if (creature.element == Element.Air)
                    {
                        damage *= 2;
                    }
                    //Fire loses to water
                    if (creature.element == Element.Water)
                    {
                        damage /= 2;
                    }
                }
                if (Spell.Element == Element.Lightning)
                {
                    //Lightning beats water
                    if (creature.element == Element.Water)
                    {
                        damage *= 2;
                    }
                    //Lightning loses to earth
                    if (creature.element == Element.Earth)
                    {
                        damage /= 2;
                    }
                }
                if (Spell.Element == Element.Air)
                {
                    //Air beats earth
                    if (creature.element == Element.Earth)
                    {
                        damage *= 2;
                    }
                    //Air loses to fire
                    if (creature.element == Element.Fire)
                    {
                        damage /= 2;
                    }
                }
                creature.TakeDamage(damage);

                if (ChainTimesRemaining > 0)
                {
                    ChainTimesRemaining--;
                    var newCreature = FindObjectsByType<CreatureBase>(FindObjectsSortMode.None)
                        .Where(c => c.Team != Spell.Team && c != creature)
                        .Where(c => (c.transform.position - transform.position).sqrMagnitude < ChainSqrRadius)
                        .OrderBy(_ => Random.value).FirstOrDefault();

                    if (newCreature != null)
                    {
                        _rb.velocity = 2 * Spell.ComputedStats.ProjectileSpeed *
                                       (newCreature.transform.position - transform.position).normalized;
                        TravelDistance -= (newCreature.transform.position - transform.position).magnitude;
                    }
                }
                else if (PiercesRemaining > 0)
                {
                    PiercesRemaining--;
                }
                else
                {
                    Dissipate();
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Room"))
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
            if (ExplodeRad > 0 && !dissipated)
            {
                dissipated = true;

                var explosionProjectile = Instantiate(
                    SpellProjectileOverseer.SpellProjectilePrefab,
                    transform.position,
                    transform.rotation
                );

                explosionProjectile.Spell = new Spell(new SpellStats
                {
                    DamageOnHit = 50,
                    CastCooldown = 0,
                    ManaUsage = 0,
                    ProjectileSpeed = 0,
                    Range = 1,
                }, Spell.Element, Spell.Team);

                explosionProjectile.AliveTime = AliveTime;
                explosionProjectile.PiercesRemaining = 999;
                explosionProjectile.GetComponent<BoxCollider2D>().enabled = false;

                explosionProjectile.CastDirection = new Vector3(0, 0, 0);
                explosionProjectile.transform.localScale += new Vector3(ExplodeRad, ExplodeRad, 0);
            }

            _rb.simulated = false;
            _particleSystem.Stop();
            Destroy(gameObject, _particleSystem.main.startLifetime.constantMax);
        }

        private void OnDestroy()
        {
            if (Overseer != null)
            {
                Overseer.RemoveProjectile(this);
            }
        }
    }
}
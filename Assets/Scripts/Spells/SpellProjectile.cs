using System;
using System.Collections.Generic;
using System.Linq;
using Creature;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spells
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(ParticleSystem))]
    public class SpellProjectile : MonoBehaviour
    {
        private static IDictionary<Element, Sprite> _elementSprites = new Dictionary<Element, Sprite>();
        
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

        public Vector3 PrevPlayerPos;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponent<ParticleSystem>();
            
            if (_elementSprites.Count == 0)
            {
                _elementSprites = new Dictionary<Element, Sprite>
                {
                    { Element.None, Resources.Load<Sprite>("Sprites/elementNeutral") },
                    { Element.Fire, Resources.Load<Sprite>("Sprites/elementFIRE") },
                    { Element.Water, Resources.Load<Sprite>("Sprites/elementWater") },
                    { Element.Air, Resources.Load<Sprite>("Sprites/elementAIR") },
                    { Element.Lightning, Resources.Load<Sprite>("Sprites/elementElectricty") },
                    { Element.Earth, Resources.Load<Sprite>("Sprites/elementearth") }
                };
            }
        }

        private void Start()
        {
            StartLive = Time.time;

            dissipated = false;

            if (BarrierSize > 0) {
                transform.localScale += (Vector3) (Vector2.Perpendicular(CastDirection) * BarrierSize);
            }

            if (OrbitRadius > 0) {
                _rb.velocity = new Vector3(Spell.ComputedStats.ProjectileSpeed, 0, 0);
                transform.position += (Vector3.down * OrbitRadius) + (Vector3.down * 0.5f);
                PrevPlayerPos = Overseer.transform.position;
            } else {
                _rb.velocity = Spell.ComputedStats.ProjectileSpeed * CastDirection;
            }

            // var main = _particleSystem.main;

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
            
            _particleSystem.textureSheetAnimation.SetSprite(0, _elementSprites[Spell.Element]);

            if (GiantSize > 0)
            {
                _rb.transform.localScale += new Vector3(GiantSize,GiantSize,0);
            }

            //main.startColor = new Color(Random.Range(0.75f, 1f), Random.Range(0.75f, 1f), Random.Range(0.75f, 1f));
        }

        private void FixedUpdate() {
            if (OrbitRadius > 0) {
                // TODO - Slight offset when moving around, no clue why
                _rb.velocity = Quaternion.Euler(0,0, Mathf.Rad2Deg * Time.deltaTime * Spell.ComputedStats.ProjectileSpeed / OrbitRadius) * _rb.velocity;
                transform.position += Overseer.transform.position - PrevPlayerPos;
                PrevPlayerPos = Overseer.transform.position;
            }
        }

        private void Update()
        {
            if (AliveTime > 0 && Time.time - StartLive >= AliveTime)
            {   
                Dissipate();
                return;
            }

            TravelDistance += _rb.velocity.magnitude * Time.deltaTime;

            if (TornadoPower > 0) {
                var creaturesToPull = FindObjectsByType<CreatureBase>(FindObjectsSortMode.None).Where(c => c.Team != Spell.Team).Where(c => (c.transform.position - transform.position).sqrMagnitude < TornadoRadius * TornadoRadius);
                foreach (var creature in creaturesToPull) {
                    creature.transform.position = Vector3.MoveTowards(creature.transform.position, transform.position, TornadoPower * Time.deltaTime);
                }
            }

            if (HomingAngle > 0)
            {
                var newCreature = FindObjectsByType<CreatureBase>(FindObjectsSortMode.None).Where(c => c.Team != Spell.Team).OrderBy(c => (c.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                if (newCreature != null)
                {
                    float angle = Vector2.SignedAngle(_rb.velocity, newCreature.transform.position - transform.position);

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
            if (other.TryGetComponent(out SpellProjectile proj) && BarrierSize > 0) {
                if (proj.Spell.Team == Spell.Team) return;
                proj.Dissipate();
            }
            if (other.TryGetComponent(out CreatureBase creature))
            {
                if (creature.Team == Spell.Team) return; // better safe than sorry
                creature.TakeDamage(Spell.ComputedStats.DamageOnHit);

                if (ChainTimesRemaining > 0)
                {
                    ChainTimesRemaining--;
                    var newCreature = FindObjectsByType<CreatureBase>(FindObjectsSortMode.None).Where(c => c.Team != Spell.Team && c != creature).Where(c => (c.transform.position - transform.position).sqrMagnitude < ChainSqrRadius).OrderBy(c => Random.value).FirstOrDefault();

                    if (newCreature != null)
                    {
                        _rb.velocity = 2 * Spell.ComputedStats.ProjectileSpeed * (newCreature.transform.position - transform.position).normalized;
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

                var Explosion = Instantiate(SpellProjectileOverseer.SpellProjectilePrefab, transform.position, transform.rotation);
                Explosion.Spell = new Spell(new SpellStats
                {
                    DamageOnHit = 1000,
                    CastCooldown = 0,
                    ManaUsage = 0,
                    ProjectileSpeed = 0,
                    Range = 1,
                }, Element.Fire, Spell.Team);
                Explosion.AliveTime = AliveTime;
                Explosion.PiercesRemaining = 999;
                Explosion.GetComponent<BoxCollider2D>().enabled = false;

                Explosion.CastDirection = new Vector3 (0, 0, 0);
                Explosion.transform.localScale += new Vector3(ExplodeRad, ExplodeRad, 0);

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
using System;
using Core;
using Spells;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Creature
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(TrailRenderer))]
    public class Player : CreatureBase
    {
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _dashAction;
        private InputAction _castAction;

        [SerializeField] protected float moveSpeed;

        [Header("Dashing")] [SerializeField] private float dashPower;

        [FormerlySerializedAs("dashCooldown")] public float DashCooldown;

        public float LastDashTime;
        public bool CanDash => Time.time - LastDashTime > DashCooldown;

        [HideInInspector] public UnityEvent OnDash = new();

        [Header("Spell-casting")] [SerializeField]
        private int _activeSpellSlot;

        public float ManaRegenPerSecond = 15f;
        public float ManaRegenStartDelay = 0.5f;
        private float _lastManaReductionTime;
        
        [field: SerializeField] public float mana { get; private set; }
        [field: SerializeField] public float maxMana { get; private set; }

        [HideInInspector] public UnityEvent<float, float> OnManaChanged = new();

        // Awake is called even before Start() is called.
        protected override void Awake()
        {
            base.Awake();

            Locator.ProvidePlayer(this);

            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Movement"];
            _dashAction = _playerInput.actions["Dash"];
            _castAction = _playerInput.actions["Cast"];

            _dashAction.performed += Dash;
            _castAction.performed += Cast;

            LastDashTime = -DashCooldown;

            /*var initSpell = new Spell(new SpellStats
            {
                DamageOnHit = 10,
                CastCooldown = 1,
                ManaUsage = 10,
                ProjectileSpeed = 20,
                Range = 10,
            }, Element.None, Team);

            //initSpell.AddModifier(SpellModifier.AllModifiers.Find(m => m.Tier == ModifierTier.Tier1 && m.Name == "Multishot"));
            //initSpell.AddModifier(SpellModifier.AllModifiers.Find(m => m.Tier == ModifierTier.Tier1 && m.Name == "Bounce"));
            initSpell.AddModifier(SpellModifier.AllModifiers.Find(m => m.Tier == ModifierTier.Tier1 && m.Name == "Chain"));

            Inventory.MoveSpellToEquipped(0, initSpell);*/
        }

        protected override void Start()
        {
            base.Start();
            SetMana(maxMana);

            Core.Locator.LevelManager.PlayerLevelUp.AddListener(() =>
            {
                SetMaxHealth(maxHealth + Core.Locator.LevelManager.getMaxHealthIncreasePerLevelUp());
                SetMaxMana(maxMana + Core.Locator.LevelManager.getMaxManaIncreasePerLevelUp());
            });
    }

    public void SetMana(float value)
        {
            mana = Mathf.Clamp(value, 0, maxMana);
            OnManaChanged.Invoke(mana, maxMana);
        }

        public void SetMaxMana(float value, bool refill = false)
        {
            var diff = value - maxMana;
            maxMana = value;
            if (refill)
            {
                SetMana(mana + diff);
            }
            else
            {
                OnManaChanged.Invoke(mana, maxMana);
            }
        }
        
        private void Update()
        {
            if (mana < maxMana && Time.time - _lastManaReductionTime > ManaRegenStartDelay)
            {
                SetMana(mana + ManaRegenPerSecond * Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            const int acceleration = 50;

            var move = _moveAction.ReadValue<Vector2>();
            if (Rb.velocity.magnitude < moveSpeed)
            {
                Rb.AddForce(move * (moveSpeed * acceleration), ForceMode2D.Force);
                Rb.velocity = Vector2.ClampMagnitude(Rb.velocity, moveSpeed);
            }

            // Walking Sound Effect
            if (AudioManager.Instance.sfxSource.isPlaying == false && Rb.velocity.sqrMagnitude > 0)
            {
                AudioManager.Instance.PlaySFX("Walking");
            }
            else if (AudioManager.Instance.sfxSource.isPlaying == true && Rb.velocity.sqrMagnitude <= 1)
            {
                AudioManager.Instance.sfxSource.Stop(); // Stops all SFX - Not ideal
            }

            UpdateMoveDir(move, move.magnitude > 0);
        }

        public void Cast(InputAction.CallbackContext context)
        {
            var activeSpell = Locator.Inventory.GetEquippedSpell(_activeSpellSlot);

            if (activeSpell != null && context.performed && activeSpell.CooldownOver && mana >= activeSpell.ComputedStats.ManaUsage)
            {
                TriggerAttackAnim();
                
                SetMana(mana - activeSpell.ComputedStats.ManaUsage);
                _lastManaReductionTime = Time.time;

                // Get direction from player to mouse
                var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePos.z = transform.position.z;
                var dir = (mousePos - transform.position).normalized;

                var overseer = gameObject.AddComponent<SpellProjectileOverseer>();
                overseer.Spell = activeSpell;
                overseer.CastDirection = dir;

                activeSpell.LastCastTime = Time.time;
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            //0.01f instead of 0 because Rb.velocity.magnitude when player is standing still is ~10^-13
            if (context.performed && CanDash && Rb.velocity.magnitude > 0.01f)
            {
                Dash();
            }
        }

        protected override void Die()
        {
            // could animate then delay for a few seconds
            SceneManager.LoadScene("EndScene");
        }

        private void Dash()
        {
            LastDashTime = Time.time;
            OnDash.Invoke();

            var move = _moveAction.ReadValue<Vector2>();
            Rb.AddForce(dashPower * move.normalized, ForceMode2D.Impulse);
            AudioManager.Instance.PlaySFX("Air Attack"); // Play SFX, Currently Air Attack
        }

        private void OnDisable()
        {
            _dashAction.performed -= Dash;
            _castAction.performed -= Cast;
        }
    }
}
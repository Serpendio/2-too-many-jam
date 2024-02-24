using Core;
using Spells;
using Spells.Modifiers;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("Dashing")] [SerializeField] private float dashPower;

        [SerializeField] private float dashTrailDuration;
        [SerializeField] private float dashCooldown;
        // private TrailRenderer _dashTrail;

        private float lastDashTime;
        private bool canDash => Time.time - lastDashTime > dashCooldown;

        [Header("Spell-casting")] [SerializeField]
        private int _activeSpellSlot;

        [SerializeField] public int mana;
        [SerializeField] public int maxMana;

        public Inventory Inventory = new();
        
        // Awake is called even before Start() is called.
        protected override void Awake()
        {
            base.Awake();
            
            Locator.ProvidePlayer(this);

            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Movement"];
            _dashAction = _playerInput.actions["Dash"];
            _castAction = _playerInput.actions["Cast"];

            // _dashTrail = GetComponent<TrailRenderer>();

            _dashAction.performed += Dash;
            _castAction.performed += Cast;

            mana = maxMana;
            
            lastDashTime = -dashCooldown;

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

        // https://files.catbox.moe/835ck8.png

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
            var activeSpell = Inventory.GetEquippedSpell(_activeSpellSlot);

            if (context.performed && activeSpell is { CooldownOver: true })
            {
                TriggerAttackAnim();
                
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
            if (context.performed && canDash && Rb.velocity.magnitude > 0.01f) //0.01f instead of 0 because Rb.velocity.magnitude when player is standing still is ~10^-13
            {
                Dash();
            }
        }

        private void Dash()
        {
            lastDashTime = Time.time;

            var move = _moveAction.ReadValue<Vector2>();
            Rb.AddForce(dashPower * move.normalized, ForceMode2D.Impulse);
            AudioManager.Instance.PlaySFX("Air Attack"); // Play SFX, Currently Air Attack
        }
    }
}
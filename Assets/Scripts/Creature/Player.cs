using Spells;
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
        public SpellProjectile SpellProjectilePrefab;

        // Awake is called even before Start() is called.
        protected override void Awake()
        {
            base.Awake();

            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Movement"];
            _dashAction = _playerInput.actions["Dash"];
            _castAction = _playerInput.actions["Cast"];

            // _dashTrail = GetComponent<TrailRenderer>();

            _dashAction.performed += Dash;
            _castAction.performed += Cast;

            mana = maxMana;

            var initSpell = new Spell(new SpellStats
            {
                DamageOnHit = 10,
                CastCooldown = 1,
                ManaUsage = 10,
                ProjectileSpeed = 20,
                Range = 10,
            }, Element.None, Team.Friendly);

            Inventory.MoveSpellToEquipped(0, initSpell);
        }

        // https://files.catbox.moe/835ck8.png

        private void FixedUpdate()
        {
            const int acceleration = 50;

            var move = _moveAction.ReadValue<Vector2>();
            if (Rb.velocity.magnitude < speed)
            {
                Rb.AddForce(move * (speed * acceleration), ForceMode2D.Force);
                Rb.velocity = Vector2.ClampMagnitude(Rb.velocity, speed);
            }

            UpdateMoveDir(move);
        }

        public void Cast(InputAction.CallbackContext context)
        {
            var activeSpell = Inventory.GetEquippedSpell(_activeSpellSlot);

            if (context.performed && activeSpell.CooldownOver)
            {
                // Get direction from player to mouse
                var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePos.z = transform.position.z;
                var dir = (mousePos - transform.position).normalized;

                var spellProjectile = Instantiate(SpellProjectilePrefab, Rb.position, Quaternion.identity);
                spellProjectile.Spell = activeSpell;
                spellProjectile.CastDirection = dir;

                activeSpell.LastCastTime = Time.time;
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            if (context.performed && canDash)
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
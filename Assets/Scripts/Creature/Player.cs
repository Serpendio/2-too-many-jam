using System.Collections.Generic;
using System.Linq;
using Core;
using Spells;
using Spells.Modifiers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Creature
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : CreatureBase
    {
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _dashAction;
        private InputAction _castAction;
        private InputAction _openInventoryAction;
        private InputAction _closeInventoryAction;
        private InputAction _hotbarSlot1Action;
        private InputAction _hotbarSlot2Action;
        private InputAction _hotbarSlot3Action;
        private InputAction _hotbarSlot4Action;
        private InputAction _hotbarSlot5Action;
        private InputAction _hotbarSlot6Action;

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

        public Spell ActiveSpell => Locator.Inventory.GetHotbarSlot(_activeSpellSlot);

        public UnityEvent<int> OnHotbarSlotChanged = new();

        // Awake is called even before Start() is called.
        protected override void Awake()
        {
            base.Awake();

            Locator.ProvidePlayer(this);

            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Movement"];
            _dashAction = _playerInput.actions["Dash"];
            _castAction = _playerInput.actions["Cast"];

            _openInventoryAction = _playerInput.actions["OpenInventory"];
            _closeInventoryAction = _playerInput.actions["CloseInventory"];

            _hotbarSlot1Action = _playerInput.actions["HotbarSlot1"];
            _hotbarSlot2Action = _playerInput.actions["HotbarSlot2"];
            _hotbarSlot3Action = _playerInput.actions["HotbarSlot3"];
            _hotbarSlot4Action = _playerInput.actions["HotbarSlot4"];
            _hotbarSlot5Action = _playerInput.actions["HotbarSlot5"];
            _hotbarSlot6Action = _playerInput.actions["HotbarSlot6"];

            _dashAction.performed += Dash;
            // _castAction.performed += AttemptCast;
            _openInventoryAction.performed += OpenInventory;
            _closeInventoryAction.performed += CloseInventory;

            _hotbarSlot1Action.performed += _ => SetActiveSpellSlot(0);
            _hotbarSlot2Action.performed += _ => SetActiveSpellSlot(1);
            _hotbarSlot3Action.performed += _ => SetActiveSpellSlot(2);
            _hotbarSlot4Action.performed += _ => SetActiveSpellSlot(3);
            _hotbarSlot5Action.performed += _ => SetActiveSpellSlot(4);
            _hotbarSlot6Action.performed += _ => SetActiveSpellSlot(5);

            LastDashTime = -DashCooldown;
        }

        protected override void Start()
        {
            base.Start();
            SetMana(maxMana);

            Locator.LevelManager.OnPlayerLevelUp.AddListener(_ =>
            {
                SetMaxHealth(maxHealth + Locator.LevelManager.getMaxHealthIncreasePerLevelUp(), true);
                SetMaxMana(maxMana + Locator.LevelManager.getMaxManaIncreasePerLevelUp(), true);
            });

            for (var i = 0; i < 3; i++)
            {
                var modifiers = new List<SpellModifier>();

                int numModifiers = Random.value <= 0.8f ? 1 : 2; //80% chance of 1 modifier, 20% chance of 2 modifiers
                for (int k = 0; k < numModifiers; ++k)
                {
                    var randomNewModifier = SpellModifier.AllModifiers
                        .Where(m => !modifiers.Contains(m))
                        .OrderBy(_ => Random.value)
                        .FirstOrDefault();

                    if (randomNewModifier == null) break;

                    modifiers.Add(randomNewModifier);
                }

                var randSpell = new Spell(new SpellStats
                {
                    DamageOnHit = 5,
                    ManaUsage = 10,
                    Range = 10,
                    CastCooldown = 1,
                    ProjectileSpeed = 10,
                    Spread = 0
                }, (Element)Random.Range(1, 6), Team.Friendly, modifiers);

                Locator.Inventory.AddToHotbar(randSpell);
            }
        }

        public void SetActiveSpellSlot(int slot)
        {
            if (slot < 0 || slot >= Locator.Inventory.MaxEquippedSpells) return;
            if (Locator.Inventory.GetHotbarSlot(slot) == null) return;

            _activeSpellSlot = slot;
            OnHotbarSlotChanged.Invoke(slot);
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

            if (Input.GetMouseButton((int)MouseButton.Left))
            {
                AttemptCast();
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
            else if (AudioManager.Instance.sfxSource.isPlaying && Rb.velocity.sqrMagnitude <= 1)
            {
                AudioManager.Instance.sfxSource.Stop(); // Stops all SFX - Not ideal
            }

            UpdateMoveDir(move, move.magnitude > 0);
        }

        public void Dash(InputAction.CallbackContext context)
        {
            //0.01f instead of 0 because Rb.velocity.magnitude when player is standing still is ~10^-13
            if (context.performed && CanDash && Rb.velocity.magnitude > 0.01f)
            {
                LastDashTime = Time.time;
                OnDash.Invoke();

                var move = _moveAction.ReadValue<Vector2>();
                Rb.AddForce(dashPower * move.normalized, ForceMode2D.Impulse);
                AudioManager.Instance.PlaySFX("Air Attack"); // Play SFX, Currently Air Attack
            }
        }

        public void AttemptCast()
        {
            if (ActiveSpell == null) return;

            if (ActiveSpell.CooldownOver && mana >= ActiveSpell.ComputedStats.ManaUsage)
            {
                TriggerAttackAnim();

                SetMana(mana - ActiveSpell.ComputedStats.ManaUsage);
                _lastManaReductionTime = Time.time;

                // Get direction from player to mouse
                var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePos.z = transform.position.z;
                var dir = (mousePos - transform.position).normalized;

                var overseer = gameObject.AddComponent<SpellProjectileOverseer>();
                overseer.Spell = ActiveSpell;
                overseer.CastDirection = dir;

                ActiveSpell.LastCastTime = Time.time;
            }
        }

        public void OpenInventory(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _playerInput.SwitchCurrentActionMap("UI");
            }
        }

        public void CloseInventory(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _playerInput.SwitchCurrentActionMap("Player");
            }
        }

        protected override void Die()
        {
            // could animate then delay for a few seconds
            SceneManager.LoadScene("EndScene");
        }

        private void OnDisable()
        {
            _dashAction.performed -= Dash;
            // _castAction.performed -= AttemptCast;

            _openInventoryAction.performed -= OpenInventory;
            _closeInventoryAction.performed -= CloseInventory;
        }
    }
}
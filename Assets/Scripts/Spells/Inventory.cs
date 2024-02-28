using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    [Serializable]
    public class Inventory
    {
        public int hotbarSize = 3;
        public const int MaxHotbarSize = 6;

        [SerializeField] private List<Spell> _equippedSpells;
        private List<IInventoryItem> _items = new();

        private void Awake() {
            _equippedSpells = new(hotbarSize) { null, null, null };
        }

        private void Start() {
            Core.Locator.LevelManager.PlayerLevelUp.AddListener(() =>
            {
                //Update max hotbar size every maxLevel / maxHotbarSize levels to ensure even distribution between level ups
                //(e.g. every 5 levels for max level = 30, max hotbar size = 6)
                if (Core.Locator.LevelManager.getCurrentLevel() % (Core.Locator.LevelManager.getMaxLevel() / hotbarSize) == 0) {
                    hotbarSize += 1;
                }
            });
        }

        public void AddToInventory(IInventoryItem item)
        {
            _items.Add(item);
        }

        public void RemoveFromInventory(IInventoryItem item)
        {
            _items.Remove(item);
        }

        public void MoveSpellToEquipped(int slot, Spell spell)
        {
            if (slot < 0 || slot >= hotbarSize)
                throw new Exception($"Invalid spell slot, must be between 0 and {hotbarSize}.");

            if (_equippedSpells[slot] != null)
            {
                MoveSpellToInventory(slot);
            }

            _items.Remove(spell);
            _equippedSpells[slot] = spell;
        }

        public void MoveSpellToInventory(int slot)
        {
            if (slot < 0 || slot >= hotbarSize)
                throw new Exception($"Invalid spell slot, must be between 0 and {hotbarSize}.");

            _items.Add(_equippedSpells[slot]);
            _equippedSpells[slot] = null;
        }

        public Spell GetEquippedSpell(int activeSpellSlot)
        {
            if (activeSpellSlot < 0 || activeSpellSlot >= hotbarSize)
                throw new Exception($"Invalid spell slot, must be between 0 and {hotbarSize}.");
            return _equippedSpells[activeSpellSlot];
        }
    }
}
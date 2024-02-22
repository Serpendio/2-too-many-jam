using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    [Serializable]
    public class Inventory
    {
        public const int MaxEquippedSpells = 3;

        [SerializeField] private List<Spell> _equippedSpells = new(MaxEquippedSpells) { null, null, null };
        private List<IInventoryItem> _items = new();

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
            if (slot is < 0 or >= MaxEquippedSpells)
                throw new Exception($"Invalid spell slot, must be between 0 and {MaxEquippedSpells}.");

            if (_equippedSpells[slot] != null)
            {
                MoveSpellToInventory(slot);
            }

            _items.Remove(spell);
            _equippedSpells[slot] = spell;
        }

        public void MoveSpellToInventory(int slot)
        {
            if (slot is < 0 or >= MaxEquippedSpells)
                throw new Exception($"Invalid spell slot, must be between 0 and {MaxEquippedSpells}.");

            _items.Add(_equippedSpells[slot]);
            _equippedSpells[slot] = null;
        }

        public Spell GetEquippedSpell(int activeSpellSlot)
        {
            if (activeSpellSlot is < 0 or >= MaxEquippedSpells)
                throw new Exception($"Invalid spell slot, must be between 0 and {MaxEquippedSpells}.");
            return _equippedSpells[activeSpellSlot];
        }
    }
}
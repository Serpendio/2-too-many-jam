using System.Collections.Generic;
using System.Linq;
using Core;
using Spells;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory
{
    public class CurrencyStore
    {
        public int GoldAmount { get; private set; }

        public readonly UnityEvent<int> OnGoldChanged = new();

        public void AddGold(int val)
        {
            GoldAmount += val;
            OnGoldChanged.Invoke(GoldAmount);
        }
    }

    public class Inventory : MonoBehaviour
    {
        public readonly CurrencyStore Currency = new();
        public readonly List<IInventoryItem> Items = new();

        public List<Spell> Hotbar => Items
            .FindAll(i => i is Spell { IsOnHotbar: true })
            .ConvertAll(i => (Spell)i);

        public readonly UnityEvent<IInventoryItem> OnItemAdded = new();
        public readonly UnityEvent<IInventoryItem> OnItemRemoved = new();

        public readonly UnityEvent<Spell> OnHotbarItemAdded = new();
        public readonly UnityEvent<Spell> OnHotbarItemRemoved = new();

        public int MaxInventorySlots;
        public int MaxEquippedSpells;

#if UNITY_EDITOR
        [SerializeField] private List<Spell> _initialDebugSpells = new();
#endif

        private void Awake()
        {
            MaxEquippedSpells = Locator.GameplaySettingsManager.InitialMaxEquippedSpells;
            MaxInventorySlots = Locator.GameplaySettingsManager.InitialMaxInventorySlots;

#if UNITY_EDITOR
            foreach (var spell in _initialDebugSpells) AddToHotbar(spell);
#endif

            Locator.ProvideInventory(this);
        }

        public int GetUnusedGridIndex()
        {
            for (var i = 0; i < MaxInventorySlots; i++)
            {
                var itemUsingIndex = Items
                    .Where(item => item is not Spell { IsOnHotbar: true })
                    .ToList()
                    .FindIndex(item => item.GridIndex == i) > -1;
                
                if (!itemUsingIndex) return i;
            }

            return -1;
        }
        
        public int GetUnusedHotbarIndex()
        {
            for (var i = 0; i < MaxEquippedSpells; i++)
            {
                var itemUsingIndex = Items
                    .Where(item => item is Spell { IsOnHotbar: true })
                    .ToList()
                    .FindIndex(item => item.GridIndex == i) > -1;
                
                if (!itemUsingIndex) return i;
            }

            return -1;
        }

        public void AddToInventory(IInventoryItem item)
        {
            if (item is Spell { IsOnHotbar: true } spell) spell.IsOnHotbar = false;

            if (item.GridIndex == -1 || Items.FindIndex(i => i.GridIndex == item.GridIndex) != -1)
            {
                item.GridIndex = GetUnusedGridIndex();
            }

            // no space in inventory
            if (item.GridIndex == -1)
            {
                Debug.LogWarning(
                    $"No more space in inventory, item {item.Name} not added! Shouldn't have been able to do that!");
                return;
            }

            Items.Add(item);
            OnItemAdded.Invoke(item);
        }

        public void RemoveFromInventory(IInventoryItem item)
        {
            Items.Remove(item);
            OnItemRemoved.Invoke(item);
        }
        
        public void AddToHotbar(Spell spell)
        {
            if (spell.GridIndex == -1 || Hotbar.FindIndex(i => i.GridIndex == spell.GridIndex) != -1)
            {
                spell.GridIndex = GetUnusedHotbarIndex();
            }

            // no space in hotbar
            if (spell.GridIndex == -1)
            {
                Debug.LogWarning(
                    $"No more space in hotbar, adding spell {spell.Name} to inventory instead.");
                AddToInventory(spell);
                return;
            }

            spell.IsOnHotbar = true;
            Items.Add(spell);
            OnHotbarItemAdded.Invoke(spell);
        }

        // public void Combine()
        // {
        //     if (_items[_invSlots[0].RelatedSlot] is Spell spell)
        //     {
        //         AddToInventory(spell);
        //         RemoveFromInventory(_invSlots[0].RelatedSlot);
        //         RemoveFromInventory(_invSlots[1].RelatedSlot);
        //     }
        // }

        public Spell GetHotbarSlot(int activeSpellSlot) => Hotbar.Find(s => s.GridIndex == activeSpellSlot);
    }
}
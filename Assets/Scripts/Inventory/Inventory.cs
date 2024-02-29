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
        public int ShardAmount { get; private set; }
        public readonly UnityEvent<int> OnSpellShardChanged = new();

        public void AddGold(int val)
        {
            GoldAmount += val;
            OnGoldChanged.Invoke(GoldAmount);
        }

        public void AddSpellShards(int val) {
            ShardAmount += val;
            OnSpellShardChanged.Invoke(ShardAmount);
        }
    }

    public class Inventory : MonoBehaviour
    {
        public readonly CurrencyStore Currency = new();
        public readonly List<IInventoryItem> Items = new();

        public List<Spell> Hotbar => Items
            .FindAll(i => i is Spell { IsOnHotbar: true })
            .ConvertAll(i => (Spell)i);

        public readonly UnityEvent<IInventoryItem> OnItemUpdate = new();

        public int MaxInventorySlots;
        public int MaxEquippedSpells;

#if UNITY_EDITOR
        // unity doesnt run constructors for inspector variables because fuck me i guess so no more of this
        // [SerializeField] private List<Spell> _initialDebugSpells = new();
#endif

        private void Awake()
        {
            MaxEquippedSpells = Locator.GameplaySettingsManager.InitialMaxEquippedSpells;
            MaxInventorySlots = Locator.GameplaySettingsManager.InitialMaxInventorySlots;

#if UNITY_EDITOR
            // foreach (var spell in _initialDebugSpells) AddToHotbar(spell);
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
                Debug.LogError($"No more space in inventory, item {item.Name} not added!");
                return;
            }

            Items.Add(item);
            OnItemUpdate.Invoke(item);
        }

        public void RemoveFromInventory(IInventoryItem item)
        {
            Items.Remove(item);
            OnItemUpdate.Invoke(item);
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
                Debug.LogWarning($"No more space in hotbar, adding spell {spell.Name} to inventory instead.");
                AddToInventory(spell);
                return;
            }

            spell.IsOnHotbar = true;
            Items.Add(spell);
            OnItemUpdate.Invoke(spell);
        }

        public void CombineSpells(Spell spellA, Spell spellB)
        {
            if (spellA == null || spellB == null) return;

            // Debug.Log($"Combining {spellA.Name} with {spellB.Name}");
            var combinedSpell = spellA.CombinedWith(spellB);
            RemoveFromInventory(spellA);
            RemoveFromInventory(spellB);

            // Debug.Log($"Created {combinedSpell.Name}");
            AddToHotbar(combinedSpell);
        }

        public Spell GetHotbarSlot(int activeSpellSlot) => Hotbar.Find(s => s.GridIndex == activeSpellSlot);
    }
}
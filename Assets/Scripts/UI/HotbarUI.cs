using System.Collections.Generic;
using Core;
using UnityEngine;

namespace UI
{
    public class HotbarUI : MonoBehaviour
    {
        private static InventorySlot _inventorySlotPrefab;

        private List<InventorySlot> _slots = new();

        private void Awake()
        {
            _inventorySlotPrefab = Resources.Load<InventorySlot>("Prefabs/UI/InventorySlot");
        }

        private void Start()
        {
            BuildBar();
            PopulateSlots();

            Locator.Inventory.OnHotbarItemAdded.AddListener(_ =>
            {
                BuildBar();
                PopulateSlots();
            });
            
            Locator.Inventory.OnHotbarItemRemoved.AddListener(_ =>
            {
                BuildBar();
                PopulateSlots();
            });
            
            Locator.Player.OnHotbarSlotChanged.AddListener(_ =>
            {
                BuildBar();
                PopulateSlots();
            });
        }

        private void BuildBar()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            _slots.Clear();
            
            for(var i = 0; i < Locator.Inventory.MaxEquippedSpells; i++)
            {
                var slot = Instantiate(_inventorySlotPrefab, transform);
                slot.Group = InventoryGroup.Hotbar;
                slot.SetItem(null);
                _slots.Add(slot);
            }
        }

        private void PopulateSlots()
        {
            var hotbarItems = Locator.Inventory.Hotbar;
            
            foreach (var item in hotbarItems) _slots[item.GridIndex].SetItem(item);
        }
    }
}
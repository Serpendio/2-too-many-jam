using System.Collections.Generic;
using System.Linq;
using Inventory;
using Spells;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        private static InventorySlot _inventorySlotPrefab;
        [SerializeField] private GridLayoutGroup _grid;

        private List<InventorySlot> _slots = new();
        
        private bool _isVisible;

        private void Awake()
        {
            _inventorySlotPrefab = Resources.Load<InventorySlot>("Prefabs/UI/InventorySlot");
        }

        public void ToggleVisibility(bool visible)
        {
            _isVisible = !visible;
            ToggleVisibility();
        }

        public void ToggleVisibility()
        {
            _isVisible = !_isVisible;

            gameObject.SetActive(_isVisible);
            Time.timeScale = _isVisible ? 0 : 1;

            if (_isVisible)
            {
                BuildGrid();
                PopulateSlots();
            }
        }

        private void BuildGrid()
        {
            foreach (Transform child in _grid.transform) Destroy(child.gameObject);
            _slots.Clear();

            for (var i = 0; i < Core.Locator.Inventory.MaxInventorySlots; i++)
            {
                var slot = Instantiate(_inventorySlotPrefab, _grid.transform);
                slot.Group = InventoryGroup.Items;
                _slots.Add(slot);
            }
        }

        private void PopulateSlots()
        {
            var items = Core.Locator.Inventory.Items;
            foreach (var item in items)
            {
                if (item is Spell { IsOnHotbar: true }) continue;
                _slots[item.GridIndex].SetItem(item);
            }
        }
    }
}
using System.Collections.Generic;
using Spells;
using Tweens;
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

        [SerializeField] private Transform _hotbarUI;

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

            var hotbarRt = (RectTransform)_hotbarUI;
            hotbarRt.pivot = _isVisible ? new Vector2(1f, 0f) : new Vector2(0.5f, 0);
            hotbarRt.anchorMin = _isVisible ? new Vector2(1f, 0f) : new Vector2(0.5f, 0);
            hotbarRt.anchorMax = _isVisible ? new Vector2(1f, 0f) : new Vector2(0.5f, 0);
            
            hotbarRt.gameObject.AddTween(new AnchoredPositionTween
            {
                from = hotbarRt.anchoredPosition,
                to = new Vector2(_isVisible ? -80 : 0, 32),
                duration = 0.2f,
                easeType = EaseType.CubicOut,
                useUnscaledTime = true
            });
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
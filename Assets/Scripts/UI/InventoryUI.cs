using System.Collections.Generic;
using Core;
using Inventory;
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

        public InventorySlot MixSlotA;
        public InventorySlot MixSlotB;

        public Button MixButton;

        private void Awake()
        {
            _inventorySlotPrefab = Resources.Load<InventorySlot>("Prefabs/UI/InventorySlot");
            ToggleVisibility(false);
            
            Locator.Inventory.OnItemUpdate.AddListener(_ =>
            {
                BuildGrid();
                PopulateSlots();
            });

            MixSlotA.OnItemChanged.AddListener(OnMixSlotChanged);
            MixSlotB.OnItemChanged.AddListener(OnMixSlotChanged);

            MixButton.onClick.AddListener(() =>
            {
                if (MixSlotA.Item is Spell spellA && MixSlotB.Item is Spell spellB)
                {
                    MixSlotA.SetItem(null);
                    MixSlotB.SetItem(null);
                    
                    Locator.Inventory.CombineSpells(spellA, spellB);
                }
            });
        }

        private void OnMixSlotChanged(IInventoryItem item)
        {
            if (MixSlotA.Item is Spell spellA && MixSlotB.Item is Spell spellB)
            {
                MixButton.interactable = spellA != spellB && spellA.Element == spellB.Element;
            }
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

            for (var i = 0; i < Locator.Inventory.MaxInventorySlots; i++)
            {
                var slot = Instantiate(_inventorySlotPrefab, _grid.transform);
                slot.Group = InventoryGroup.Items;
                _slots.Add(slot);
            }
        }

        private void PopulateSlots()
        {
            var items = Locator.Inventory.Items;
            foreach (var item in items)
            {
                if (item is Spell { IsOnHotbar: true }) continue;
                _slots[item.GridIndex].SetItem(item);
            }
        }
    }
}